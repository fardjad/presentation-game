import Vue from "vue";
import Vuex from "vuex";
import { Commit } from "vuex/types";

import {
  ActionPayload,
  TypedMutationPayload,
  SetSlideNumberPayload,
  SetElapsedTimePayload,
  SetKeywordStatePayload,
  TypedStoreOptions,
  Action,
  SetSlidesPayload,
  SetStatePayload,
  SetLikertScaleQuestionValuePayload
} from "./store.d";
import { State, Slide } from "./model";
import { Dispatcher } from "@/utils/Dispatcher";
import { Reader } from "@/utils/Reader";
import ReconnectingWebSocket from "reconnecting-websocket";
import axios from "axios";
import { BASE_URL, WATCH_URL } from "./constants";

const httpClient = axios.create({
  baseURL: BASE_URL
});

Vue.use(Vuex);

const buildAtomicSyncAction = (actionName: Action) => (
  { commit }: { commit: Commit },
  payload: ActionPayload
): void => {
  commit(actionName, payload);
};

class TypedStore extends Vuex.Store<State> {
  constructor(options: TypedStoreOptions) {
    super(options);
  }
}

const store = new TypedStore({
  state: {
    slides: {},
    elapsedTime: 0,
    slideNumber: 0
  },
  mutations: {
    setSlideNumber(
      state: State,
      { payload: { slideNumber } }: TypedMutationPayload<SetSlideNumberPayload>
    ) {
      state.slideNumber = slideNumber;
    },
    setElapsedTime(
      state: State,
      { payload: { elapsedTime } }: TypedMutationPayload<SetElapsedTimePayload>
    ) {
      state.elapsedTime = elapsedTime;
    },
    setKeywordState(
      state: State,
      {
        payload: { slideIndex, text, said }
      }: TypedMutationPayload<SetKeywordStatePayload>
    ) {
      state.slides = {
        ...state.slides,
        [slideIndex]: {
          ...state.slides[slideIndex],
          keywords: { ...state.slides[slideIndex].keywords, [text]: said }
        }
      };
    },
    setLikertScaleQuestionValue(
      state: State,
      {
        payload: { slideIndex, questionIndex, value }
      }: TypedMutationPayload<SetLikertScaleQuestionValuePayload>
    ) {
      state.slides = {
        ...state.slides,
        [slideIndex]: {
          ...state.slides[slideIndex],
          likertScaleQuestions: state.slides[
            slideIndex
          ].likertScaleQuestions.map(
            (question, index) =>
              index === questionIndex
                ? {
                    ...question,
                    value
                  }
                : question
          )
        }
      };
    },
    setSlides(
      state: State,
      { payload: { slides } }: TypedMutationPayload<SetSlidesPayload>
    ) {
      state.slides = slides;
    },
    setState(
      state: State,
      {
        payload: { slides, elapsedTime, slideNumber }
      }: TypedMutationPayload<SetStatePayload>
    ) {
      state.slides = slides;
      state.elapsedTime = elapsedTime;
      state.slideNumber = slideNumber;
    }
  },
  actions: {
    setSlideNumber: buildAtomicSyncAction("setSlideNumber"),
    setElapsedTime: buildAtomicSyncAction("setElapsedTime"),
    setKeywordState: buildAtomicSyncAction("setKeywordState"),
    setLikertScaleQuestionValue: buildAtomicSyncAction(
      "setLikertScaleQuestionValue"
    ),
    setSlides: buildAtomicSyncAction("setSlides"),
    setState: buildAtomicSyncAction("setState"),
    async fetchSlideNumber() {
      const response = await httpClient.get("/state/slidenumber");
      setSlideNumberDispatcher.dispatch(store, {
        slideNumber: response.data
      });
    },
    async fetchElapsedTime() {
      const response = await httpClient.get("/state/elapsedtime");
      setElapsedTimeDispatcher.dispatch(store, {
        elapsedTime: response.data
      });
    },
    async fetchSlides() {
      const response = await httpClient.get("/state/slides");
      setSlidesDispatcher.dispatch(store, {
        slides: response.data
      });
    },
    async fetchState() {
      const response = await httpClient.get("/state");
      setStateDispatcher.dispatch(store, response.data);
    },
    async remoteSetKeywordState(
      { dispatch },
      { payload: { slideIndex, text, said } }
    ) {
      const response = await httpClient.put(
        `/state/slides/${slideIndex}/${text}`,
        {
          value: said
        }
      );
      if (response.status !== 204) {
        dispatch("fetchSlides");
      } else {
        setKeywordStateDispatcher.dispatch(store, {
          slideIndex,
          text,
          said
        });
      }
    },
    async remoteSetLikertScaleQuestionValue(
      { dispatch },
      { payload: { slideIndex, questionIndex, value } }
    ) {
      const response = await httpClient.put(
        `/state/slides/${slideIndex}/likertscalequestions/${questionIndex}`,
        {
          value
        }
      );
      if (response.status !== 204) {
        dispatch("fetchSlides");
      } else {
        setLikertScaleQuestionValueDispatcher.dispatch(store, {
          slideIndex,
          questionIndex,
          value
        });
      }
    }
  }
});

export const setSlideNumberDispatcher = new Dispatcher<SetSlideNumberPayload>(
  "setSlideNumber"
);

export const setElapsedTimeDispatcher = new Dispatcher<SetElapsedTimePayload>(
  "setElapsedTime"
);

export const setKeywordStateDispatcher = new Dispatcher<SetKeywordStatePayload>(
  "setKeywordState"
);

export const setLikertScaleQuestionValueDispatcher = new Dispatcher<
  SetLikertScaleQuestionValuePayload
>("setLikertScaleQuestionValue");

export const setSlidesDispatcher = new Dispatcher<SetSlidesPayload>(
  "setSlides"
);

export const remoteSetKeywordStateDispatcher = new Dispatcher<
  SetKeywordStatePayload
>("remoteSetKeywordState");

export const remoteSetLikertScaleQuestionValueDispatcher = new Dispatcher<
  SetLikertScaleQuestionValuePayload
>("remoteSetLikertScaleQuestionValue");

export const slidesReader = new Reader<"slides", Slide[]>("slides");
export const elapsedTimeReader = new Reader<"elapsedTime", number>(
  "elapsedTime"
);
export const slideNumberReader = new Reader<"slideNumber", number>(
  "slideNumber"
);

export const getCurrentSlide = ({ state }: TypedStore) => {
  return state.slides[state.slideNumber];
};

export const setStateDispatcher = new Dispatcher<SetStatePayload>("setState");
export const fetchStateDispatcher = new Dispatcher<ActionPayload>("fetchState");

const ws = new ReconnectingWebSocket(WATCH_URL);
ws.addEventListener("message", async ({ data }: { data: string }) => {
  const mapper: { [key in keyof State]: (data: any) => void } = {
    slides: (slides: any) =>
      setSlidesDispatcher.dispatch(store, {
        slides
      }),
    elapsedTime: (elapsedTime: any) =>
      setElapsedTimeDispatcher.dispatch(store, {
        elapsedTime
      }),
    slideNumber: (slideNumber: any) =>
      setSlideNumberDispatcher.dispatch(store, {
        slideNumber
      })
  };
  const dispatcher = mapper[data as keyof State];
  if (dispatcher) {
    const response = await httpClient.get(`/state/${data}`);
    if (response.status === 200) {
      dispatcher(response.data);
    }
  }
});
ws.addEventListener("open", () => fetchStateDispatcher.dispatch(store, {}));

export default store;
