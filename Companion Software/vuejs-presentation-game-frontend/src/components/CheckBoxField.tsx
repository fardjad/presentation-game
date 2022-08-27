import { Vue, Component, Prop, Model, Emit } from "vue-property-decorator";

@Component
export default class CheckBoxField extends Vue {
  @Prop()
  private text!: string;

  @Prop()
  private checked!: boolean;

  private onChange(checked: boolean): void {
    this.$emit("change", checked);
  }

  private render() {
    return (
      <div class="panel-block">
        <div class="columns flex-1">
          <div class="column is-2">
            <label v-uni-for="checkbox" class="label">
              {this.text}
            </label>
          </div>
          <div class="column">
            <dd>
              <input
                v-uni-id="checkbox"
                type="checkbox"
                checked={this.checked}
                onChange={({ target: { checked } }: { target: any }) =>
                  this.onChange(checked)
                }
              />
            </dd>
          </div>
        </div>
      </div>
    );
  }
}
