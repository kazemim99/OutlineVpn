import Vue from "vue";
import Vuetify from "vuetify/lib/framework";

// Translation provided by Vuetify (javascript)
import fa from "vuetify/lib/locale/fa";

// Translation provided by Vuetify (typescript)
import pl from "vuetify/src/locale/pl";

// Your own translation file

import "@mdi/font/css/materialdesignicons.css"; // Ensure you are using css-loader
Vue.use(Vuetify);

export default new Vuetify({
  lang: {
    locales: { fa, pl },
    current: "fa",
  },
  icons: {
    iconfont: "mdi", // 'mdi' || 'mdiSvg' || 'md' || 'fa' || 'fa4' || 'faSvg'
  },
  rtl: true,
  customVariables: ["@/assets/sass/_variables.scss"],
});
