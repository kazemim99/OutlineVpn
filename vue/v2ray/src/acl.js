//get user permissions
//if you are using token or something you could do something like
import Vue from "vue";
import store from "@/store/index";

//create v-can directive
Vue.directive("can", {
  bind: function (el, binding, vnode) {
    // let perm = JSON.parse(localStorage.getItem("permissions")).includes(
    //   binding.value.replace(/'/g, "").replace(/"/g, "")
    // );
    if (binding.value == "false") {
      vnode.elm.style.display = "none";
    }
  },
});
