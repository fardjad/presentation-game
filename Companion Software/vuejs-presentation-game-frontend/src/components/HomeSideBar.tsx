import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class HomeSideBar extends Vue {
  @Prop()
  private msg!: string;

  protected render() {
    return (
      <aside class="menu">
        <ul class="menu-list">
          <li>
            <router-link to="/home/slides">Slides</router-link>
          </li>
        </ul>
      </aside>
    );
  }
}
