import request from "@/utils/request";
import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
Vue.use(Vuex);

export default new Vuex.Store({
  plugins: [createPersistedState()],
  state: {
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
    setSelectedComplexName(state, selectedComplex: "") {
      state.selectedComplexName = selectedComplex;
    },
    setSelectedComplexId(state, selectedComplex: any) {
      state.selectedComplexId = selectedComplex;
    },
    emptyComplexRoles(state) {
      state.complexRoles = [];
    },
    setComplexRoles(state, complexRoles: []) {
      state.complexRoles = complexRoles;
    },
    setRoles(state, roles: []) {
      state.roles = roles;
    },
    setUserDetails(state, userDetails: any) {
      state.userDetails = userDetails;
    },
    setUserStates(state, userStates: any) {
      state.userStates = userStates;
    },
    setComplexes(state, complexes: any) {
      state.complexes = complexes;
    },
    setSelectedDeviceName(state, name: string) {
      state.selectedDeviceName = name;
    },
  },
  actions: {
    getPermissions({ commit }) {
      request.get("/userRoleAndPermission/permissions").then((response) => {
        commit("setPermissions", response.data.result);
      });
    },
    getComplexRoles({ commit }) {
      request.get(`/complexRole/roles`).then((response) => {
        commit("setComplexRoles", response.data.result);
      });
    },
    getComplexes({ commit }) {
      request.get("/complex/complexes-tree").then((response) => {
        commit("setComplexes", response.data.result);
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
