import { getToken } from "@/utils/cookies";
import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import Dashboard from '@/layout/Dashboard.vue'
import MainMenu from '@/layout/MainMenu.vue'
import store from "@/store";

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/dashboard/',
    name: 'Dashboard',
    component: Dashboard,
    redirect: '/buy-vpn',
    meta: { requireAuth: true },
    children: [
      {
        path: 'buy-vpn',
        name: 'BuyKey',
        component: () => import('@/views/HomePage/getKey.vue'),
        meta: { requireAuth: true }
      },

      {
        path: 'swaps',
        name: 'Swap',
        component: () => import('@/views/Swaps/swap.vue'),
        meta: { requireAuth: true }
      },
      {
        path: 'users',
        name: 'User',
        component: () => import('@/views/User/Users.vue'),
        meta: { requireAuth: true }
      },
      {
        path: 'manage-plans',
        name: 'ManagPlans',
        component: () => import('@/views/Plans/ManagePlans.vue'),
        meta: { requireAuth: true }
      },
      {
        path: "problem-reports",
        name: "ProblemReports",
        component: () => import("@/views/ProblemReports/index.vue"),
      },
      {
        path: "orders",
        name: "Orders",
        component: () => import("@/views/Orders/index.vue"),
      },

      {
        path: "phone-toturial",
        name: "IPhoneAndroidToturial",
        component: () => import("@/views/HomePage/IPhoneAndroidToturial.vue"),
        meta: { requireAuth: true },
      },
      {
        path: "windows-toturial",
        name: "Windows",
        meta: { requireAuth: true },

        component: () => import("@/views/HomePage/Windows.vue"),
      },
      {
        path: "linux-toturial",
        name: "Linux",
        meta: { requireAuth: true },

        component: () => import("@/views/HomePage/Linux.vue"),
      },
      {
        path: "sshkeys",
        name: "SSHKeys",
        meta: { requireAuth: true },
        component: () => import("@/views/SSHKeys/index.vue"),
      },

      {
        path: "profile",
        name: "Profile",
        component: () => import("@/views/User/Profile.vue"),
        meta: { requireAuth: true }
      },

      // {
      //   path: "checkout/:id",
      //   name: "Checkout",
      //   component: () => import("@/views/Plans/Checkout.vue"),
      //   meta: { requireAuth: true }
      // },

      // {
      //   path: "plans",
      //   name: "Plans",
      //   component: () => import("@/views/Plans/index.vue"),
      //   meta: { requireAuth: true }
      // },

      // {
      //   path: "v2servers",
      //   name: "V2Servers",
      //   component: () => import("@/views/V2Servers/index.vue"),
      //   meta: { requireAuth: true }
      // },
      // {
      //   path: "v2Keys",
      //   name: "v2Keys",
      //   component: () => import("@/views/V2Keys/index.vue"),
      //   meta: { requireAuth: true }
      // },

    ]
  },
  {

    path: '/',
    name: 'Home',
    component: () => import('@/views/Home.vue'),
    meta: {
      title: 'فیلتر شکن - فروش فیلتر شکن - فیلتر شکن ایفون , اندروید , کامپیوتر , ویندوز',
      metaTags: [
        {
          name: 'description',
          content: 'فروش فیلتر شکن پر سرعت  برای اندورید , ایفون , ویندوز , کامپیوتر , لینوکس همراه با مهلت تست و پشتیبانی 24 ساعته'
        },
        {
          property: 'og:description',
          content: 'فروش فیلتر شکن پر سرعت  برای اندورید , ایفون , ویندوز , کامپیوتر , لینوکس همراه با مهلت تست و پشتیبانی 24 ساعته'
        }
      ]
    }
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
  // const nearestWithTitle = to.matched.slice().reverse().find(r => r.meta && r.meta.title);

  // if (nearestWithTitle) document.title = nearestWithTitle.meta.title;

  if (to.matched.some((record) => record.meta.requireAuth) && !isLoggedIn) {
    next({ path: '/login', query: { returnUrl: to.path } });
  }

  next();
});
export default router;