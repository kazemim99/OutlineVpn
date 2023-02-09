import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
import VueSweetalert2 from 'vue-sweetalert2';
import acl from '../src/acl.js';
import './assets/sass/main.scss';
import VueCompositionAPI from '@vue/composition-api'


Vue.config.productionTip = false


Vue.use(VueCompositionAPI)

Vue.use(VueSweetalert2)
import 'sweetalert2/dist/sweetalert2.min.css';

import VuePersianDatetimePicker from 'vue-persian-datetime-picker';
Vue.component('date-picker', VuePersianDatetimePicker);

Vue.directive(acl)
new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
