<template>
  <v-app dark>
    <v-navigation-drawer v-model="drawer" fixed app right>
      <v-list>
        <v-list-item>
          <v-list-item-avatar>
            <v-img src="~@/assets/images/avatar.jpeg"></v-img>
          </v-list-item-avatar>
        </v-list-item>

        <v-list-item link>
          <v-list-item-content>
            <v-list-item-title class="mb-3">{{
              currentUser
            }}</v-list-item-title>
            <!-- <v-list-item-subtitle>Lorem</v-list-item-subtitle> -->
          </v-list-item-content>
        </v-list-item>
      </v-list>
      <v-divider></v-divider>

      <v-list nav>
        <template v-for="(item, i) in items">
          <v-list-item
            v-can="`${item.permission}`"
            v-if="!item.items"
            :key="i"
            :to="item.to"
            router
            exact
            color="primary"
          >
            <v-list-item-action>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title v-text="item.title"></v-list-item-title>
            </v-list-item-content>
          </v-list-item>

          <v-list-group v-else :key="i">
            <template v-slot:activator>
              <v-list-item-action>
                <v-icon>{{ item.icon }}</v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title v-text="item.title"></v-list-item-title>
              </v-list-item-content>
            </template>
            <template v-for="subItem in item.items">
              <v-list-item
                v-can="`${subItem.permission}`"
                :key="subItem.title"
                :to="subItem.to"
                router
                exact
                color="primary"
              >
                <v-list-item-action>
                  <v-icon size="medium" class="pl-4">{{ subItem.icon }}</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title v-text="subItem.title"></v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>
          </v-list-group>
        </template>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar fixed app>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title v-text="title" />
      <v-spacer></v-spacer>

      <v-btn icon @click="getNotif()">
        <v-icon>mdi-bell-ring</v-icon>
        <v-badge color="green" content="15"> </v-badge>
      </v-btn>

      <v-btn icon @click="logout">
        <v-icon>mdi-logout</v-icon>
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-container>
        <router-view :key="$route.path"></router-view>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";
@Component({
  name: "Dashboard",
})
export default class Dashboard extends Vue {
  private drawer = true;
  private title = "پاور باکس";
  private fixed = false;
  getNotif() {
    this.$router.push("/device-notifications");
  }
  private items = [
    {
      icon: "mdi-home",
      title: "صفحه اصلی",
      to: "/home",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Home_Show",
    },
    {
      icon: "mdi-account",
      title: "مدیریت کاربران",
      adminVisible: true,
      permission: "Member_Show",
      to: "/users",
    },

    {
      icon: "mdi-domain",
      title: "مجموعه ها",
      adminVisible: true,
      to: "/complexes",
      permission: "Complex_Show",
    },

    {
      icon: "mdi-account-key ",
      title: "نقش ها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Role_Show",
      to: "/complex-roles",
    },
    {
      icon: "mdi-newspaper-variant-outline ",
      title: "گزارش بازدید فنی",
      permission: "TechnicalReport_Show",
      to: "/reports",
      adminVisible: true,
    },

    {
      icon: "mdi-account-check ",
      title: "حضور و غیاب",
      to: "/attendance",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Attendance_Show",
    },
    {
      icon: "mdi-card-text  ",
      title: "متن اعلان ها",
      to: "/notifications",
      permission: "NotifContent_Show",
      adminVisible: this.$store.state.userDetails.isAdmin,
    },
    {
      icon: "mdi-bell-ring",
      title: "اعلان ها",
      to: "/device-notifications",
      permission: "Notif_Show",
      adminVisible: true,
    },
    {
      icon: "mdi-lifebuoy ",
      title: "پشتیبانی",
      permission: "Support_Show",
      to: "/support",
      adminVisible: this.$store.state.userDetails.isAdmin,
    },
    {
      icon: "mdi-lifebuoy ",
      title: "تصویر امنیتی",
      permission: "SecurityImage_Show",
      to: "/security-image",
      adminVisible: true,
    },
    {
      icon: "",
      title: "گزارش استفاده",
      to: "/hourse-charge-filter",
      permission: "",
    },
    {
      icon: "",
      title: "گزارش اکسل",
      to: "/excel-export",
      permission: "Excel_Show",
    },
    {
      icon: "",
      title: "لاگ کاربران",
      to: "/member-logs",
      permission: "Log_Show",
    },
    {
      icon: "mdi-chart-line",
      title: "آمارها",
      to: "",
      permission: "Chart_Show",
      items: [
        {
          icon: "",
          title: "استفاده کابل شارژ",
          to: "/chart-charge",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "انتخاب عکس امنیتی",
          to: "/chart-pic-pass",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "پر استفاده ترین دستگاه",
          to: "/chart-device-uses",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "امتیاز کاربران",
          to: "/chart-rate",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: " ساعات استفاده",
          to: "/chart-user-hours",
          permission: "Chart_Show",
        },

        {
          icon: "",
          title: "اپراتورهای تلفن همراه",
          to: "/chart-operator",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "استان ها",
          to: "/chart-provinces",
          permission: "Chart_Show",
        },
        // {
        //   icon: "",
        //   title: "کاربران یکتا",
        //   to: "/chart-unique-user",
        //   permission: "Chart_Show",
        // },
        {
          icon: "",
          title: "پراکندی کاربران یکتا",
          to: "/per-unique-user-per-day-per-terminal",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "پراکندگی کاربران",
          to: "/per-user-per-day-per-terminal",
          permission: "Chart_Show",
        },
        {
          icon: "",
          title: "پراکندگی ساعات شارژ",
          to: "/charge-duration",
          permission: "Chart_Show",
        },
        //   {
        //   icon: "",
        //   title: "پراکندگی کاربران یکتا ",
        //   to: "/per-unique-day-per-terminal",
        //   permission: "Chart_Show",
        // }

        // {
        //   icon: "",
        //   title: "درصد کاربران مجموعه ",
        //   to: "/chart-user-percent",
        //   permission: "Chart_Show",
        // },
      ],
    },
    {
      icon: "mdi-account-edit",
      title: "پروفایل",
      permission: "Profile_Show",
      to: "/profile",
      adminVisible: true,
    },
  ];
  get currentUser() {
    return UserModule.fullName;
  }

  private logout = () => {
    UserModule.ResetToken();
    localStorage.removeItem("permissions");
    location.reload();
  };
}
</script>
<style scoped>
</style>
