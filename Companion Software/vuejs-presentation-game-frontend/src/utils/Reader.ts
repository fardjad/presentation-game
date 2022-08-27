import { Store } from "vuex/types";
import { State } from "@/model";

type StateKey = keyof State;
type StateValue<T extends StateKey> = State[T];

export class Reader<T extends StateKey, U extends StateValue<T>> {
  private key: T;

  constructor(key: T) {
    this.key = key;
  }
  public read(store: Store<State>) {
    return store.state[this.key] as U;
  }
}
