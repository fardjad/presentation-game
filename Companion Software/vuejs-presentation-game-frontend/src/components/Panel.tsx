import { Vue, Component, Prop } from "vue-property-decorator";

@Component
export default class Panel extends Vue {
  @Prop()
  private heading!: string;
  private render() {
    return (
      <div class="wrapper has-text-left">
        <div class="panel">
          <p class="panel-heading">{this.heading}</p>
          {this.$slots.default}
        </div>
      </div>
    );
  }
}
