import { Vue, Component, Prop } from "vue-property-decorator";

@Component
export default class StaticField extends Vue {
  @Prop()
  private label!: string;
  @Prop()
  private text!: string;
  private render() {
    return (
      <div class="panel-block">
        <div class="columns flex-1">
          <div class="column is-2">
            <dt class="label">{this.label}</dt>
          </div>
          <div class="column">
            <dd>{this.text}</dd>
          </div>
        </div>
      </div>
    );
  }
}
