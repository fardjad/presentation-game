import { Store } from "vuex/types";
import { ActionPayload, Action } from "@/store.d";
import { State } from "@/model";

export class Dispatcher<T extends ActionPayload> {
  private type: string;

  constructor(type: Action) {
    this.type = type;
  }
  public dispatch(store: Store<State>, payload: T) {
    store.dispatch({
      type: this.type,
      payload
    });
  }
}
