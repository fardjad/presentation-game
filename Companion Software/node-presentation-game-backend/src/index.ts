import cors from "cors";
import express from "express";
import expressWs from "express-ws";
import { Either } from "fp-ts/lib/Either";
import { identity } from "fp-ts/lib/function";
import { IO } from "fp-ts/lib/IO";
import { fromEither, fromIO, TaskEither, taskify } from "fp-ts/lib/TaskEither";
import fs, { PathLike } from "fs";
import path from "path";
import uuidv4 from "uuid/v4";
import WebSocket from "ws";
import moment from 'moment';
import player from 'node-wav-player';

import {
  compose,
  concatEither,
  notNull,
  parseJSON,
  parseNumber,
  toBoolean,
  toNumber,
  toPositiveNumber,
  toString,
  valueOrDefault
} from "./fp-utils";
import * as openapi from "./openapi.json";
import zmqServer from "./zmqServer";

interface LikertScaleQuestions {
  readonly text: string;
  readonly value?: number;
}

interface Slide {
  readonly index: number;
  readonly keywords: { readonly [text: string]: boolean };
  readonly likertScaleQuestions: ReadonlyArray<LikertScaleQuestions>;
  readonly uri: string;
  readonly shouldRaiseHand: boolean;
}

// tslint:disable:readonly-keyword
interface Model {
  slides: { readonly [index: number]: Slide };
  elapsedTime: number;
  slideNumber: number;
  questionsStartIndex: number;
  isInQaTime: boolean;
  slideEvents: {from: number, to: number, ts: number}[];
  startEvent: number;
}
// tslint:enable:readonly-keyword

// tslint:disable-next-line:no-let
let watchers: ReadonlyArray<WebSocket> = [];

const PORT: number = compose(
  valueOrDefault("8080"),
  either => either.chain(parseNumber)
)(process.env.PORT).fold(identity, identity);

const ZMQ_PORT: number = compose(
  valueOrDefault("3000"),
  either => either.chain(parseNumber)
)(process.env.ZMQ_PORT).fold(identity, identity);

const HOST: string = compose(
  valueOrDefault("0.0.0.0"),
  either => either.chain(toString)
)(process.env.HOST).fold(identity, identity);

const fileName = path.join(__dirname, `../data/${moment().format('YYYY-MM-DD[T]HH-mm-ss[Z]ZZ')}-${uuidv4()}.json`);

const { app } = expressWs(express());
app.use(express.json());
app.use(cors());
app.use("/slides", express.static(path.join(__dirname, "../slides")));

const state: Model = {
  elapsedTime: 0,
  isInQaTime: false,
  questionsStartIndex: 0,
  slideNumber: 0,
  slides: {},
  slideEvents: [],
  startEvent: 0
};

const readFile = taskify(fs.readFile) as (
  path: PathLike
) => TaskEither<Error, Buffer>;

const writeFile = taskify(fs.writeFile) as (
  path: PathLike,
  data: string
) => TaskEither<Error, void>;

readFile(path.join(__dirname, "../slides/model.json"))
  .map(buffer => buffer.toString("utf8"))
  .chain(str => fromEither(parseJSON<Model>(str)))
  .run()
  .then(either => {
    either.fold(
      err => {
        console.error(err);
      },
      model => {
        // tslint:disable:no-object-mutation
        state.elapsedTime = model.elapsedTime;
        state.isInQaTime = model.isInQaTime;
        state.questionsStartIndex = model.questionsStartIndex;
        state.slideNumber = model.slideNumber;
        state.slides = model.slides;
        // tslint:enable:no-object-mutation
      }
    );
  });

const wsSend = (ws: WebSocket) =>
  taskify(ws.send.bind(ws)) as (data: string) => TaskEither<Error, void>;

const setState = (nextState: Model) => {
  // tslint:disable:no-object-mutation

  if (state['slideNumber'] !== nextState['slideNumber']) {
    state.slideEvents.push({
      from: state['slideNumber'],
      to: nextState['slideNumber'],
      ts: Date.now()
    });
  }

  (Object.keys(state) as ReadonlyArray<keyof Model>).forEach(key => {
    if (state[key] !== nextState[key]) {
      state[key] = nextState[key];
      Promise.all(
        watchers
          .map(wsSend)
          .map(send => send(key))
          .map(task => task.run())
      );
    }
  });

  if (state.slideNumber >= state.questionsStartIndex) {
    state.isInQaTime = true;
  }

  writeFile(fileName, JSON.stringify(state, null, 2))
    .run()
    .then(errorOrVoid => {
      errorOrVoid.fold(console.error, _ => _);
    });

  // tslint:enable:no-object-mutation
};

const toSlide = (key: number) =>
  notNull(new Error("Invalid slide number"))(state.slides[key]) as Either<
    Error,
    Slide
  >;

const toSlideIndex = (key: number) => toSlide(key).map(_ => key);

const toQuestionIndex = (questionIndex: number) => (slide: Slide) =>
  notNull(new Error("Invalid slide number"))(
    slide.likertScaleQuestions[questionIndex]
  ).map(_ => questionIndex);

const toKeyword = (keyword: string) => (slide: Slide) =>
  notNull(new Error("Invalid keyword"))(slide.keywords[keyword]).map(
    _ => keyword
  );

const lowerCasedKeys = (Object.keys(state) as ReadonlyArray<keyof Model>)
  .map(k => ({
    [k.toLowerCase()]: k
  }))
  .reduce((acc, obj) => ({ ...acc, ...obj }));

app.get("/status", (req, res) => {
  res.status(200).json({
    status: "OK"
  });
});

app.get("/openapi.json", (req, res) => {
  res.status(200).json(openapi);
});

app.get("/state", (req, res) => {
  res.status(200).json(state);
});

app.get("/state/:key", (req, res) => {
  const key = (req.params.key as string).toLowerCase();

  const errorOrValue: Either<Error, Model[keyof Model]> = compose(
    notNull(new Error("Invalid key!")),
    e => e.map((k: keyof Model) => state[k])
  )(lowerCasedKeys[key]);

  errorOrValue.fold(
    error => {
      res.status(404).json({
        message: error.message
      });
    },
    value => {
      res.status(200).json(value);
    }
  );
});

app.put("/state/elapsedtime", (req, res) => {
  const errorOrValue: Either<Error, number> = compose(
    toNumber,
    v => v.chain(toPositiveNumber)
  )(req.body.value);

  errorOrValue.fold(
    error => {
      res.status(400).json({
        message: error.message
      });
    },
    value => {
      setState({ ...state, elapsedTime: value });
      res.status(204).end();
    }
  );
});

app.put("/state/slidenumber", (req, res) => {
  const errorOrValue: Either<Error, number> = compose(
    notNull(new Error("Invalid slide number!")),
    v => v.chain(toSlideIndex)
  )(req.body.value);

  errorOrValue.fold(
    error => {
      res.status(400).json({
        message: error.message
      });
    },
    value => {
      setState({ ...state, slideNumber: value });
      res.status(204).end();
    }
  );
});

app.put(
  "/state/slides/:slideIndex/likertscalequestions/:questionIndex",
  (req, res) => {
    const errorOrSlideIndex: Either<Error, number> = compose(
      parseNumber,
      either => either.chain(toSlideIndex)
    )(req.params.slideIndex);

    const errorOrQuestionIndex: Either<Error, number> = compose(
      either => either.chain(toSlide),
      either => either.chain(toQuestionIndex(req.params.questionIndex)),
      either => either.chain(parseNumber),
      either => either.chain(toPositiveNumber)
    )(errorOrSlideIndex);

    const errorOrValue: Either<Error, number> = compose(
      toNumber,
      either => either.chain(toPositiveNumber)
    )(req.body.value);

    const threeTuple = concatEither(
      errorOrSlideIndex.map(v => [v]),
      errorOrQuestionIndex.map(v => [v]),
      errorOrValue.map(v => [v])
    ) as Either<Error, [number, number, number]>;

    threeTuple.fold(
      error => {
        res.status(400).json({
          message: error.message
        });
      },
      ([index, likertScaleQuestionIndex, value]) => {
        setState({
          ...state,
          slides: {
            ...state.slides,
            [index]: {
              ...state.slides[index],
              likertScaleQuestions: [
                ...Object.keys(state.slides[index].likertScaleQuestions)
                  .map(k => parseInt(k, 10))
                  .map(
                    idx =>
                      idx === likertScaleQuestionIndex
                        ? {
                            ...state.slides[index].likertScaleQuestions[idx],
                            value
                          }
                        : state.slides[index].likertScaleQuestions[idx]
                  )
              ]
            }
          }
        });

        res.status(204).end();
      }
    );
  }
);

app.put("/state/slides/:slideIndex/:keyword", (req, res) => {
  const errorOrSlideIndex: Either<Error, number> = compose(
    parseNumber,
    either => either.chain(toSlideIndex)
  )(req.params.slideIndex);

  const errorOrKeyword: Either<Error, string> = compose(
    either => either.chain(toSlide),
    either => either.chain(toKeyword(req.params.keyword))
  )(errorOrSlideIndex);

  const errorOrValue: Either<Error, boolean> = toBoolean(req.body.value);

  const threeTuple = concatEither(
    errorOrSlideIndex.map(v => [v]),
    errorOrKeyword.map(v => [v]),
    errorOrValue.map(v => [v])
  ) as Either<Error, [number, string, boolean]>;

  threeTuple.fold(
    error => {
      res.status(400).json({
        message: error.message
      });
    },
    ([index, kw, value]) => {
      setState({
        ...state,
        slides: {
          ...state.slides,
          [index]: {
            ...state.slides[index],
            keywords: {
              ...state.slides[index].keywords,
              [kw]: value
            }
          }
        }
      });

      res.status(204).end();
    }
  );
});

app.ws("/watch", (ws, req) => {
  watchers = [...watchers, ws];
  ws.once("close", () => {
    watchers = watchers.filter(item => item !== ws);
  });
});

// tslint:disable-next-line:no-expression-statement
app.listen(PORT, HOST, () => {
  console.log(`Server is listening on http://${HOST}:${PORT}`);
});

zmqServer.listen("tcp", HOST, ZMQ_PORT).then(() => {
  console.log(`ZMQ server is listening on tcp://${HOST}:${ZMQ_PORT}`);
});

zmqServer.zmqReceiver.on("message", (message: string) => {
  interface ZmqMessage {
    readonly type:
      | "slideIndex"
      | "elapsedTime"
      | "slides"
      | "currentSlideScore"
      | "start";
    readonly value?: Model[keyof Model];
  }

  const errorOrObject = parseJSON<ZmqMessage>(message);

  fromEither(errorOrObject)
    .chain(obj => {
      switch (obj.type) {
        case "start":
        return fromIO(
          new IO(() => {
            player.play({
              path: path.join(__dirname, '../sfx/tag.wav')
            }).then(() => {
              setState({...state, startEvent: Date.now()});
            });
          })
        );
        case "slideIndex":
          return fromIO(
            new IO(() => {
              const slideNumber = obj.value as number;
              setState({
                ...state,
                slideNumber
              });
              zmqServer.send(
                JSON.stringify({
                  type: "qa",
                  value: state.isInQaTime
                })
              );
            })
          );
        case "elapsedTime":
          return fromIO(
            new IO(() => {
              setState({
                ...state,
                elapsedTime: obj.value as number
              });
            })
          );
        case "slides":
          return fromIO(
            new IO(() => {
              zmqServer.send(
                JSON.stringify({
                  type: "slides",
                  value: state.slides
                })
              );
            })
          );
        case "currentSlideScore":
          return fromIO(
            new IO(() => {
              const currentSlide = state.slides[state.slideNumber];
              const keywords = currentSlide.keywords;
              zmqServer.send(
                JSON.stringify({
                  type: "currentSlideScore",
                  value: {
                    numberOfKeywords: Object.keys(keywords).length,
                    numberOfMentionedKeywords: Object.keys(keywords).filter(
                      keyword => keywords[keyword]
                    ).length
                  }
                })
              );
            })
          );
      }
    })
    .fold(console.error, identity)
    .run();
});
