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
  private title = "IRAN Outline";
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
      icon: "mdi-currency-usd",
      title: "پلن ها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "Role_Show",
      to: "/manage-plans",
    },

    {
      icon: "mdi-server",
      title: "سرورها",
      adminVisible: this.$store.state.userDetails.isAdmin,
      permission: "ApiUrl_Show",
      to: "/api-urls",
    },
    {
      icon: "mdi-currency-usd",
      title: "خرید ترافیک",
      adminVisible: true,
      permission: "Member_Show",
      to: "/buy-traffic",
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
