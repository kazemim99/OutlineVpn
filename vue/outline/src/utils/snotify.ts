import Vue from "vue";
import Snotify, { SnotifyPosition } from "vue-snotify";

Vue.use(Snotify, {
  toast: {
    position: SnotifyPosition.rightTop,
  },
});



// import Vue from "vue";
// import Snotify from "vue-snotify";
// import { SnotifyService } from "vue-snotify/SnotifyService";

// declare module "vue/types/vue" {
//   interface Vue {
//     $snotify: SnotifyService;
//   }
// }
