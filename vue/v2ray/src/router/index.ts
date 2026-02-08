import { getToken } from "@/utils/cookies";
import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import Dashboard from "@/layout/Dashboard.vue";
import MainMenu from "@/layout/MainMenu.vue";
import store from "@/store";

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: "/dashboard/",
    name: "Dashboard",
    component: Dashboard,
    redirect: "/sshkeys",
    meta: { requireAuth: true },
    children: [
      {
        path: "users",
        name: "User",
        component: () => import("@/views/User/Users.vue"),
        meta: { requireAuth: true },
      },

      {
        path: "orders",
        name: "Orders",
        component: () => import("@/views/Orders/index.vue"),
        meta: { requireAuth: true },
      },

      {
        path: "sshkeys",
        name: "SSHKeys",
        meta: { requireAuth: true },
        component: () => import("@/views/SSHKeys/index.vue"),
      },
      {
        path: "dnsrecords",
        name: "DNSRecords",
        meta: { requireAuth: true },
        component: () => import("@/views/DNSRecords/index.vue"),
      },
      {
        path: "profile",
        name: "Profile",
        component: () => import("@/views/User/Profile.vue"),
        meta: { requireAuth: true },
      },

      {
        path: "linux-toturial",
        name: "Linux",

        component: () => import("@/views/HomePage/Linux.vue"),
      },

      {
        path: "v2servers",
        name: "V2Servers",
        component: () => import("@/views/V2Servers/index.vue"),
        meta: { requireAuth: true },
      },
      // {
      //   path: "v2Keys",
      //   name: "v2Keys",
      //   component: () => import("@/views/V2Keys/index.vue"),
      //   meta: { requireAuth: true }
      // },
    ],
  },
  {
    path: "/sshkeys",
    name: "Home",
    redirect: "/dashboard/sshkeys",
    component: () => import("@/views/Home.vue"),
  },
  {
    path: "/dnsrecords",
    name: "DNSRecords",
    redirect: "/dashboard/dnsrecords",
    component: () => import("@/views/DNSRecords/index.vue"),
  },
  {
    path: "/",
    name: "CustomerLogin",
    component: () => import("@/views/Login/customerLogin.vue"),
  },
  {
    path: "/customerInfo",
    name: "CustomerInfo",
    component: () => import("@/views/Login/customerInfo.vue"),
  },
  {
    path: "/login",
    name: "Login",
    component: () => import("@/views/Login/customerLogin.vue"),
  },
  {
    path: "/phone-toturial",
    name: "IPhoneAndroidToturial",
    component: () => import("@/views/HomePage/IPhoneAndroidToturial.vue"),
  },
  {
    path: "/windows-toturial",
    name: "Windows",
    component: () => import("@/views/HomePage/Windows.vue"),
  },
  {
    path: "/get-code",
    name: "GetCode",
    component: () => import("@/views/Login/get-code.vue"),
  },
  {
    path: "/verify-code",
    name: "VerifyCode",
    component: () => import("@/views/Login/verifyCode.vue"),
  },
  {
    path: "/change-password",
    name: "ChangePassword",
    component: () => import("@/views/Login/change-password.vue"),
  },
];

const router = new VueRouter({
  mode: "history",
  base: "/",
  routes,
});

router.beforeEach((to, from, next) => {
  const isLoggedIn = getToken();
  const isAdmin = store.state.userDetails;
  // const nearestWithTitle = to.matched.slice().reverse().find(r => r.meta && r.meta.title);

  // if (nearestWithTitle) document.title = nearestWithTitle.meta.title;

  if (to.matched.some((record) => record.meta.requireAuth) && !isLoggedIn) {
    next({ path: "/", query: { returnUrl: to.path } });
  }

  next();
});
export default router;
