import { Component, Vue } from "vue-property-decorator";

@Component
export default class App extends Vue {
  protected render() {
    return (
      <div id="app">
        <nav class="navbar is-white">
          <div class="container">
            <div class="navbar-brand">
              <a class="navbar-item brand-text" href="/">
                Presentation Game
              </a>
              <div class="navbar-burger burger" data-target="navMenu">
                <span />
                <span />
                <span />
              </div>
            </div>
            <div id="navMenu" class="navbar-menu">
              <div class="navbar-start">
                <router-link
                  active-class=""
                  exact-active-class=""
                  class="navbar-item"
                  to="/home"
                >
                  Home
                </router-link>
              </div>
            </div>
          </div>
        </nav>
        <router-view />
      </div>
    );
  }
}
