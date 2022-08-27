import { Vue, Component } from "vue-property-decorator";
import HomeSideBar from "@/components/HomeSideBar.vue";

@Component
export default class Home extends Vue {
  private render() {
    return (
      <div class="container">
        <div class="columns">
          <div class="column is-3">
            <HomeSideBar />
          </div>
          <div class="column is-9">
            <router-view />
          </div>
        </div>
      </div>
    );
  }
}
