import { Vue, Component } from "vue-property-decorator";
import {
  elapsedTimeReader,
  getCurrentSlide,
  slideNumberReader,
  remoteSetKeywordStateDispatcher,
  remoteSetLikertScaleQuestionValueDispatcher
} from "@/store";
import StaticField from "@/components/StaticField.vue";
import Panel from "@/components/Panel.vue";
import CheckBoxField from "@/components/CheckBoxField.vue";
import SliderField from "@/components/SliderField.vue";
import { BASE_URL } from "@/constants";

@Component
export default class Slides extends Vue {
  private get elapsedTime() {
    return elapsedTimeReader.read(this.$store);
  }

  private get currentSlide() {
    return getCurrentSlide(this.$store);
  }

  private get slideNumber() {
    return slideNumberReader.read(this.$store);
  }

  private remoteSetKeywordState(
    slideIndex: number,
    text: string,
    said: boolean
  ) {
    return remoteSetKeywordStateDispatcher.dispatch(this.$store, {
      slideIndex,
      text,
      said
    });
  }

  private remoteSetLikertScaleQuestionValue(
    slideIndex: number,
    questionIndex: number,
    value: number
  ) {
    return remoteSetLikertScaleQuestionValueDispatcher.dispatch(this.$store, {
      slideIndex,
      questionIndex,
      value
    });
  }

  private render() {
    return (
      <div class="has-text-left">
        <Panel heading="Info">
          <StaticField label="Elapsed Time" text={this.elapsedTime} />
          <StaticField label="Slide Number" text={this.slideNumber} />
          {this.currentSlide != null ? (
            <img src={BASE_URL + this.currentSlide.uri} />
          ) : null}
        </Panel>
        <Panel heading="Keywords">
          {this.currentSlide != null
            ? Object.keys(this.currentSlide.keywords).map(key => (
                <CheckBoxField
                  text={key}
                  checked={this.currentSlide.keywords[key]}
                  onChange={(value: boolean) =>
                    this.remoteSetKeywordState(this.slideNumber, key, value)
                  }
                />
              ))
            : null}
        </Panel>
        <Panel heading="Questions">
          {this.currentSlide != null
            ? this.currentSlide.likertScaleQuestions.map((question, index) => (
                <SliderField
                  text={question.text}
                  value={question.value}
                  onChange={(value: string) =>
                    this.remoteSetLikertScaleQuestionValue(
                      this.slideNumber,
                      index,
                      parseInt(value, 10)
                    )
                  }
                />
              ))
            : null}
        </Panel>
      </div>
    );
  }
}
