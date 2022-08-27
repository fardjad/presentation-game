interface LikertScaleQuestions {
  text: string;
  value?: number;
}

export interface Slide {
  index: number;
  keywords: {
    [text: string]: boolean;
  };
  likertScaleQuestions: LikertScaleQuestions[];
  uri: string;
}

export interface State {
  slides: {
    [index: number]: Slide;
  };
  elapsedTime: number;
  slideNumber: number;
}
