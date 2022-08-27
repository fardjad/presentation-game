import Vue from "vue";
import Router from "vue-router";
import Home from "@/views/Home.vue";
import Slides from "@/views/Slides.vue";

Vue.use(Router);

export default new Router({
  linkActiveClass: "is-active",
  linkExactActiveClass: "is-exact-active",
  routes: [
    {
      path: "/",
      redirect: {
        path: "home"
      }
    },
    {
      path: "/home",
      component: Home,
      children: [
        {
          path: "",
          redirect: {
            path: "slides"
          }
        },
        {
          path: "slides",
          component: Slides
        }
      ]
    }
  ]
});
