import Vue from "vue";
import VueSwal from "vue-sweetalert2";

Vue.use(VueSwal);

export default (ctx, inject) => {
  ctx.$swal = VueSwal;
  inject("swal", VueSwal);
};
// import Vue from "vue";
// import Snotify from "vue-snotify";
// import { SnotifyService } from "vue-snotify/SnotifyService";

// declare module "vue/types/vue" {
//   interface Vue {
//     $snotify: SnotifyService;
//   }
// }
