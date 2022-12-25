import request from "@/utils/request";
import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
Vue.use(Vuex);

export default new Vuex.Store({
  plugins: [createPersistedState()],
  state: {
    userMobile: "",
    userStates: [],
    selectedDeviceName: "",
    userDetails: {
      isAdmin: false,
    },
    roles: [],
    permissions: [],
    complexes: [],
    complexRoles: [],
    selectedComplexName: "",
    selectedComplexId: null,
  },
  mutations: {

    setRoles(state, roles: []) {
      state.roles = roles;
    },
    setUserDetails(state, userDetails: any) {
      state.userDetails = userDetails;
      console.log(userDetails);
    },
    setUserStates(state, userStates: any) {
      state.userStates = userStates;
    }

  },
  actions: {


    getComplexRoles({ commit }) {
      request.get(`/complexRole/roles`).then((response) => {
        commit("setComplexRoles", response.data.result);
      });
    },

    getRoles({ commit }) {
      request.get("/userRoleAndPermission/roles").then((response) => {
        commit("setRoles", response.data.result);
      });
    },
    getUserStates({ commit }) {
      request.get("/publicdata/user-states").then((response) => {
        commit("setUserStates", response.data.result);
      });
    },
  },
  modules: {},
});
