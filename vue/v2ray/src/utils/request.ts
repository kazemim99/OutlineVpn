import axios from "axios";
import Vue from "vue";
import { getToken } from "@/utils/cookies";

const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true // send cookies when cross-domain requests
});

service.interceptors.request.use(
  (config) => {
    // Add X-Access-Token header to every request, you can add other custom headers here
    if (getToken()) {
      config.headers["Authorization"] = getToken();
    }
    return config;
  },
  (error) => {
    Promise.reject(error);
  }
);

service.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    debugger;

    if(error.response.data.detail){
      Vue.swal("خطا", error.response.data.detail, "error");
    }else{

    let message = error.response.data;
    if (message.errors) {
      const result = Object.keys(message.errors).map((key) => [
        key,
        message.errors[key],
      ]);

      for (let index = 1; index < result[0].length; index++) {
        message = result[0][index] + "<br/>";
      }
    }
    Vue.swal("خطا", message, "error");
  }
    return Promise.reject(error);
  }
);

export default service;
