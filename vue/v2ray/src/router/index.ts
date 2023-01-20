import { getToken } from "@/utils/cookies";
import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import Dashboard from '@/layout/Dashboard.vue'
import store from "@/store";

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard,
    redirect: '/home',
    meta: { requireAuth: true },
    children: [
      {
        path: '/home',
        name: 'Home',
        component: () => import('@/views/Home.vue'),
        meta: { requireAuth: true }
      },

      {
        path: '/swaps',
        name: 'Swap',
        component: () => import('@/views/Swaps/swap.vue'),
        meta: { requireAuth: true }
      },
      {
        path: '/users',
        name: 'User',
        component: () => import('@/views/User/Users.vue'),
        meta: { requireAuth: true }
      },
      {
        path: '/manage-plans',
        name: 'ManagPlans',
        component: () => import('@/views/Plans/ManagePlans.vue'),
        meta: { requireAuth: true }
      },
      {
        path: "/problem-reports",
        name: "ProblemReports",
        component: () => import("@/views/ProblemReports/index.vue"),
      },
      {
        path: "/sshkeys",
        name: "SSHKeys",
        component: () => import("@/views/SSHKeys/index.vue"),
      },

      {
        path: "/profile",
        name: "DeviceReport",
        component: () => import("@/views/User/Profile.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/checkout/:id",
        name: "Checkout",
        component: () => import("@/views/Plans/Checkout.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/buy-traffic",
        name: "BuyTraffic",
        component: () => import("@/views/Plans/index.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/plans",
        name: "Plans",
        component: () => import("@/views/Plans/index.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/v2servers",
        name: "V2Servers",
        component: () => import("@/views/V2Servers/index.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/v2Keys",
        name: "v2Keys",
        component: () => import("@/views/V2Keys/index.vue"),
        meta: { requireAuth: true }
      },

    ]
  },

  {
    path: "/login",
    name: "Login",
    component: () => import("@/views/Login/index.vue"),
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
  }
];

const router = new VueRouter({
  mode: "history",
  base: "/",
  routes,
});

router.beforeEach((to, from, next) => {
  const isLoggedIn = getToken();
  const isAdmin = store.state.userDetails;
  if (to.matched.some((record) => record.meta.requireAuth) && !isLoggedIn) {
    next({ path: '/login', query: { returnUrl: to.path } });
  }

  next();
});
export default router;