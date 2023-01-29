<template>
  <div id="app">
    <v-app id="inspire">
      <v-card color="grey lighten-4" flat height="200px" tile>
        <v-toolbar>
          <template v-for="item in items">
            <v-btn router :key="item.title" :to="item.to">
              <v-icon>{{ item.title }}</v-icon>
            </v-btn>
          </template>
        </v-toolbar>
      </v-card>
    </v-app>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { UserModule } from "@/store/modules/user";
@Component({
  name: "Dashboard",
})
export default class Dashboard extends Vue {
  private drawer = true;
  private title = "IranV2Ray";
  private fixed = false;
  getNotif() {
    this.$router.push("/device-notifications");
  }
  private items = [
    {
      icon: "mdi-home",
      title: "صفحه اصلی",
      to: "/home",
      adminVisible: true,
      permission: "Home_Show",
    },
    {
      icon: "mdi-sync",
      title: "انتقال",
      adminVisible: this.$store.state.userDetails.isAdmin,
      to: "/swaps",
    },

    {
      icon: "mdi-account",
      title: "مدیریت کاربران",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Member_Show",
      to: "/users",
    },

    {
      icon: "mdi-account-key ",
      title: "نقش ها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Role_Show",
      to: "/user-roles",
    },

    {
      icon: "mdi-currency-usd ",
      title: "تراکنش ها",
      permission: "Role_Show",
      to: "/orders",
    },
    {
      icon: "mdi-alert-box",
      title: "گزارش مشکل",
      to: "/problem-reports",
    },
    {
      icon: "mdi-server",
      title: "سرورها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "ApiUrl_Show",
      to: "/v2servers",
    },
    {
      icon: "mdi-key",
      title: "کلیدها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "ApiUrl_Show",
      to: "/sshkeys",
    },

    // {
    //   icon: "mdi-currency-usd",
    //   title: "خرید ترافیک",
    //   adminVisible: true,
    //   permission: "Member_Show",
    //   to: "/buy-traffic",
    // },
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
