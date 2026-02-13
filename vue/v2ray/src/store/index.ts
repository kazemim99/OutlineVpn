import request from "@/utils/request";
import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
Vue.use(Vuex);

export default new Vuex.Store({
  plugins: [
    createPersistedState({
      paths: ['auth', 'userDetails', 'customerUserName', 'roles', 'permissions']
    })
  ],
  state: {
    userMobile: "",
    userStates: [],
    selectedDeviceName: "",
    customerUserName: "",
    userDetails: {
      isAdmin: false,
      firstName: null,
      lastName: null,
    },
    roles: [],
    permissions: [],
  },
  mutations: {
    setRoles(state, roles: []) {
      state.roles = roles;
    },
    setUserDetails(state, userDetails: any) {
      state.userDetails = userDetails;
    },

    setCustomerUserName(state, userDetails: any) {
      state.customerUserName = userDetails;
    },
    setUserStates(state, userStates: any) {
      state.userStates = userStates;
    },
  },
  actions: {
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
