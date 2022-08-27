import { Vue, Component, Prop, Model, Watch } from "vue-property-decorator";

@Component
export default class SliderField extends Vue {
  @Prop()
  private text!: string;

  @Prop()
  private value!: number;

  private currentValue: number = 0;

  protected created() {
    this.currentValue = this.value;
  }

  protected render() {
    return (
      <div class="panel-block">
        <div class="columns flex-1">
          <div class="column is-2">
            <label v-uni-for="range" class="label">
              {this.text}
            </label>
          </div>
          <div class="column">
            <dd>
              <input
                v-uni-id="range"
                type="range"
                value={this.currentValue}
                min="0"
                max="10"
                onInput={({ target: { value } }: { target: any }) => {
                  this.currentValue = value;
                }}
                onChange={({ target: { value } }: { target: any }) =>
                  this.onChange(value)
                }
              />
            </dd>
          </div>
          <div class="column is-narrow">
            <dd>
              <span>{this.currentValue}</span>
            </dd>
          </div>
        </div>
      </div>
    );
  }

  private onChange(value: number): void {
    this.$emit("change", value);
  }

  @Watch("value")
  private onValuePropChange() {
    this.currentValue = this.value;
  }
}
