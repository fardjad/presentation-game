import {
  MutationPayload,
  MutationTree,
  ActionTree,
  GetterTree,
  StoreOptions,
  Mutation as VuexMutation,
  Action as VuexAction,
  Getter as VuexGetter
} from "vuex/types";
import { State } from "./model";

/* tslint:disable:no-empty-interface */
export interface ActionPayload {}
/* tslint:enable:no-empty-interface */

export interface SetSlideNumberPayload extends ActionPayload {
  slideNumber: number;
}

export interface SetElapsedTimePayload extends ActionPayload {
  elapsedTime: number;
}

export interface SetKeywordStatePayload extends ActionPayload {
  slideIndex: number;
  text: string;
  said: boolean;
}

export interface SetLikertScaleQuestionValuePayload extends ActionPayload {
  slideIndex: number;
  questionIndex: number;
  value: number;
}

export interface SetSlidesPayload extends ActionPayload {
  slides: State["slides"];
}

export interface SetStatePayload extends ActionPayload, State {}

export interface TypedMutationPayload<T> extends MutationPayload {
  payload: T;
}

declare enum ActionsEnum {
  setElapsedTime,
  setKeywordState,
  setLikertScaleQuestionValue,
  setSlideNumber,
  setSlides,
  setState,
  fetchState,
  fetchSlides,
  fetchElapsedTime,
  fetchSlideNumber,
  remoteSetKeywordState,
  remoteSetLikertScaleQuestionValue
}

declare enum MutationsEnum {
  setElapsedTime,
  setKeywordState,
  setLikertScaleQuestionValue,
  setSlideNumber,
  setSlides,
  setState
}

export type Action = keyof typeof ActionsEnum;
export type Mutation = keyof typeof MutationsEnum;

declare enum GettersEnum {
  currentSlide
}

type MutationTreeMappedType = { [key in Mutation]: VuexMutation<State> };
interface TypedMutationTree
  extends MutationTree<State>,
    MutationTreeMappedType {}

type ActionTreeMappedType = { [key in Action]: VuexAction<State, State> };
interface TypedActionTree
  extends ActionTree<State, State>,
    ActionTreeMappedType {}

interface TypedStoreOptions extends StoreOptions<State> {
  actions?: TypedActionTree;
  mutations?: TypedMutationTree;
}
