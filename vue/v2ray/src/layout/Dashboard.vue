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
            v-can="`${item.adminVisible}`"
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
                v-can="`${subItem.adminVisible}`"
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

      <!-- <v-btn icon @click="getNotif()">
        <v-icon>mdi-bell-ring</v-icon>
        <v-badge color="green" content="15"> </v-badge>
      </v-btn> -->

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
  private title = "IranV2Ray";
  private fixed = false;
  getNotif() {
    this.$router.push("/device-notifications");
  }
  private items = [
    {
      icon: "mdi-home",
      title: "صفحه اصلی",
      to: "/dashboard/buy-vpn",
      adminVisible: true,
      permission: "Home_Show",
    },
    {
      icon: "mdi-sync",
      title: "انتقال",
      adminVisible: this.$store.state.userDetails.isAdmin,
      to: "/dashboard/swaps",
    },

    {
      icon: "mdi-account",
      title: "مدیریت کاربران",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Member_Show",
      to: "/dashboard/users",
    },

    {
      icon: "mdi-account-key ",
      title: "نقش ها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Role_Show",
      to: "/dashboard/user-roles",
    },

    {
      icon: "mdi-currency-usd ",
      title: "تراکنش ها",
      permission: "Role_Show",
      to: "/dashboard/orders",
    },
    {
      icon: "mdi-alert-box",
      title: "آموزش اتصال با ایفون و اندروید",
      to: "/dashboard/phone-toturial",
    },
    {
      icon: "mdi-alert-box",
      title: "آموزش اتصال ویندوز",
      to: "/dashboard/windows-toturial",
    },
    {
      icon: "mdi-alert-box",
      title: "اموزش اتصال لینوکس",
      to: "/dashboard/linux-toturial",
    },
    {
      icon: "mdi-alert-box",
      title: "گزارش مشکل",
      to: "/dashboard/problem-reports",
    },
    {
      icon: "mdi-server",
      title: "سرورها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "ApiUrl_Show",
      to: "/dashboard/v2servers",
    },
    {
      icon: "mdi-key",
      title: "کلیدها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "ApiUrl_Show",
      to: "/dashboard/sshkeys",
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
      to: "/dashboard/profile",
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
