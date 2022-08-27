import { Either, left, right } from "fp-ts/lib/Either";

export const notNull = <T>(error: Error) => (
  input: T | null
): Either<Error, T> => (input == null ? left(error) : right(input));

export const valueOrDefault = <T>(defaultValue: T) => (
  input: T | null
): Either<T, T> => (input == null ? left(defaultValue) : right(input));

const notNaN = (error: Error) => (input: number): Either<Error, number> =>
  Number.isNaN(input) ? left(error) : right(input);

type NonNullPrimitive = boolean | number | string | symbol | object;

type NonNullPrimitiveAsString =
  | "boolean"
  | "number"
  | "string"
  | "symbol"
  | "object";

const ofType = (type: NonNullPrimitiveAsString) => (error: Error) => (
  input: any
): Either<Error, NonNullPrimitive> =>
  typeof input !== type ? left(error) : right(input);

type AnyFunc = (arg: any) => any;

const composeArray = (fns: ReadonlyArray<AnyFunc>) => (arg: any) =>
  fns.reduce((acc, fn) => fn(acc), arg);

// tslint:disable-next-line:readonly-array
export const compose = (...args: AnyFunc[]) => composeArray(args);

export const toNumber: (value: any) => Either<Error, number> = compose(
  notNull(new TypeError("Value is null!")),
  either => either.chain(ofType("number")(TypeError("Invalid number!"))),
  either => either.chain(notNaN(new TypeError("Value is NaN!")))
);

export const toBoolean: (value: any) => Either<Error, boolean> = compose(
  notNull(new TypeError("Value is null!")),
  either => either.chain(ofType("boolean")(TypeError("Invalid boolean!")))
);

export const toString: (value: any) => Either<Error, string> = compose(
  notNull(new TypeError("Value is null")),
  either => either.chain(ofType("string")(TypeError("Invalid string")))
);

export const parseJSON: <T>(value: any) => Either<Error, T> = compose(
  toString,
  either =>
    either.chain((str: string) => {
      try {
        return right(JSON.parse(str));
      } catch (ex) {
        return left(ex);
      }
    })
);

export const parseNumber: (value: any) => Either<Error, number> = compose(
  toString,
  either => either.map((str: string) => parseInt(str, 10)),
  v => v.chain(toNumber)
);

export const toPositiveNumber = (value: number) =>
  value >= 0 ? right(value) : left(new Error("Value is negative!"));

export const concatEitherArray = <L, R>(
  eitherArray: ReadonlyArray<Either<L, ReadonlyArray<R>>>
) =>
  eitherArray.reduce((acc, either) =>
    acc.chain(accArr => either.map(arr => [...accArr, ...arr]))
  );

// tslint:disable:readonly-array
export const concatEither = <L>(
  ...args: Array<Either<L, ReadonlyArray<any>>>
) => concatEitherArray(args);
// tslint:enable:readonly-array
