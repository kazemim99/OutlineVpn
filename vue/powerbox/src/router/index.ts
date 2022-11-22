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
        path: '/users',
        name: 'User',
        component: () => import('@/views/User/Users.vue'),
        meta: { requireAuth: true }
      },
      {
        path: "/complexes",
        name: "Complexes",
        component: () => import("@/views/Complex/Complexes.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/complex-units/:id",
        name: "ComplexUnits",
        component: () => import("@/views/Complex/ComplexUnits.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/complex-devices/:id",
        name: "ComplexDevices",
        component: () => import("@/views/Device/Devices.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/complex-roles",
        name: "ComplexPermissions",
        component: () => import("@/views/Complex/ComplexPermissions.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/complex-customers/:id",
        name: "ComplexCustomers",
        component: () => import("@/views/Complex/ComplexCustomers.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/complex-members/:id",
        name: "ComplexMembers",
        component: () => import("@/views/Complex/ComplexMembers.vue"),
        meta: { requireAuth: true }
      },

      //Device
      {
        path: "/device-advertise/:id",
        name: "DeviceAdvertise",
        component: () => import("@/views/Device/Advertises.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-code-activity/:id",
        name: "DeviceCodeActivity",
        component: () => import("@/views/Device/CodeActivity.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-sms-panel/:id",
        name: "DeviceSmsPanel",
        component: () => import("@/views/Device/DeviceSmsPanel.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-pins/:id",
        name: "DevicePins",
        component: () => import("@/views/Device/DevicePins.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/hourse-charge-filter",
        name: "HourseCharge",
        component: () => import("@/views/Device/HourseCharge.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-state/:id",
        name: "DeviceState",
        component: () => import("@/views/Device/DeviceState.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/attendance",
        name: "DeviceState",
        component: () => import("@/views/Attendance/Attendances.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/security-image",
        name: "SecurityImage",
        component: () => import("@/views/Support/SecurityImages.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-report/:id",
        name: "DeviceReport",
        component: () => import("@/views/Device/DeviceReport.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/support",
        name: "Supporte",
        component: () => import("@/views/Support/Supportes.vue"),
        meta: { requireAuth: true }
      },

      {
        path: "/profile",
        name: "DeviceReport",
        component: () => import("@/views/User/Profile.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/reports",
        name: "UserReport",
        component: () => import("@/views/Report/Reports.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/notifications",
        name: "Notifications",
        component: () => import("@/views/Notification/Notifications.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/device-notifications",
        name: "DeviceNotifications",
        component: () => import("@/views/Notification/DeviceNotifications.vue"),
        meta: { requireAuth: true }
      },
      {
        path: "/chart-charge",
        name: "ChartCharge",
        component: () => import("@/views/Chart/charge.vue"),
      },
      {
        path: "/chart-pic-pass",
        name: "PicPass",
        component: () => import("@/views/Chart/picpass.vue"),
      },
      {
        path: "/chart-device-uses",
        name: "DeviceUser",
        component: () => import("@/views/Chart/deviceUses.vue"),
      },
      {
        path: "/chart-rate",
        name: "UserRate",
        component: () => import("@/views/Chart/rate.vue"),
      },
      {
        path: "/chart-user-hours",
        name: "UserHours",
        component: () => import("@/views/Chart/userHours.vue"),
      },
      {
        path: "/chart-operator",
        name: "UserHours",
        component: () => import("@/views/Chart/operator.vue"),
      },
      {
        path: "/chart-provinces",
        name: "ChartProvnices",
        component: () => import("@/views/Chart/provinces.vue"),
      },
      {
        path: "/excel-export",
        name: "ExcelExport",
        component: () => import("@/views/Device/ExcelExport.vue"),
      },
      {
        path: "/chart-unique-user",
        name: "ChartUniqueUser",
        component: () => import("@/views/Chart/uniqueUser.vue"),
      },
      {
        path: "/chart-perday-perterminal",
        name: "PerDayPerTerminal",
        component: () => import("@/views/Chart/perDayPerTerminal.vue"),
      },
      {
        path: "/member-logs",
        name: "MemberLogs",
        component: () => import("@/views/User/Logs.vue"),
      },
      {
        path: "/chart-charge-time",
        name: "ChartChargeTime",
        component: () => import("@/views/Chart/chargeTime.vue"),
      },
      {
        path: "/per-unique-user-per-day-per-terminal",
        name: "PerUniqueUserPerDayPerTerminal",
        component: () => import("@/views/Chart/perUniqueUserPerDayPerTerminal.vue"),
      },
      {
        path: "/per-user-per-day-per-terminal",
        name: "PerUserPerDayPerTerminal",
        component: () => import("@/views/Chart/perUserPerDayPerTerminal.vue"),
      },
      {
        path: "/charge-duration",
        name: "ChargeDuration",
        component: () => import("@/views/Chart/cahrgeDuration.vue"),
      }

    ]
  },
  {
    path: "/employeeComplexes",
    name: "EmployeeComplexes",
    component: () => import("@/views/Complex/Complexes.vue"),
    meta: { requireAuth: true }
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
  if (to.matched.some((record) => record.meta.requireAuth) && !isLoggedIn) {
    next({ path: '/login', query: { returnUrl: to.path } });
  }

  next();
});
export default router;