import Vue from "vue";

import Buefy from "buefy";
import "@/assets/scss/app.scss";

import { library } from "@fortawesome/fontawesome-svg-core";
import * as freeSolidSvgIcons from "@fortawesome/free-solid-svg-icons";

import VueUniqIds from "vue-uniq-ids";

import App from "@/App.vue";
import router from "@/router";
import store from "@/store";

Vue.use(Buefy);
Vue.use(VueUniqIds);

Object.keys(freeSolidSvgIcons)
  .filter(key => key.startsWith("fa"))
  .forEach(key => library.add((freeSolidSvgIcons as any)[key]));

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render(h) {
    return h(App);
  }
}).$mount("#app");
