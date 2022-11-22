<template>
  <div>
    <v-row class="mb-4">
      <Breadcrump class="mb-2" :crumbs="crumbs" />
      <v-spacer></v-spacer>

      <v-btn
        @click="$router.go(-1)"
        class="mx-10 mt-5"
        fab
        small
        dark
        color="indigo"
      >
        <v-icon dark> mdi-arrow-left </v-icon>
      </v-btn>
    </v-row>
    <v-row>
      <v-toolbar flat>
        <v-col cols="3">
          <template right>
            <AddNewComplex
              ref="addComplexCom"
              @reloadComplexes="getComplexes"
            />
          </template>
        </v-col>
        <v-spacer></v-spacer>
        <v-divider class="mx-4" inset vertical></v-divider>

        <v-toolbar-title>{{ parentNameFa }}</v-toolbar-title>
      </v-toolbar>
      <v-spacer></v-spacer>
    </v-row>

    <v-card
      v-for="item in items"
      :key="item.id"
      class="mx-auto"
      style="margin-top: 15px; padding: 5px"
      max-width="100%"
    >
      <v-card-title primary-title>
        <div>
          <h3 class="headline mb-0">{{ item.nameFa }}</h3>
        </div>
      </v-card-title>
      <v-row>
        <v-col v-for="card in cards" :key="card.id" cols="12" sm="6" md="3">
          <v-hover v-slot="{ hover }">
            <v-card
              v-can="getPermission(card.action, item)"
              :disabled="!item.state"
              @click="getAction(item, card.action)"
              :elevation="hover ? 16 : 2"
              :class="{ 'on-hover': hover }"
              style="background-color: #00b894; cursor: pointer"
            >
              <v-img :src="card.image" height="150"> </v-img>
              <v-card-actions>
                <span
                  style="text-align: center"
                  class="text-h6 white--text d-inline-block"
                  v-text="card.name"
                ></span>
              </v-card-actions>
            </v-card>
          </v-hover>
        </v-col>
      </v-row>
      <v-card-actions>
        <v-btn color="cyan" outlined text @click="editItem(item)">
          ویرایش
        </v-btn>

        <v-btn color="error" outlined text @click="deleteItem(item.id)">
          حذف
        </v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNewComplex from "@/components/complex/AddNewComplex.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";
export default {
  name: "ComplesUnits",
  components: {
    AddNewComplex,
    Breadcrump,
  },
  data() {
    return {
      crumbs: [
        {
          text: "خانه",
          disabled: false,
          href: "/",
        },
        {
          text: "مجموعه ها",
          disabled: false,
          href: "/complexes",
        },
        {
          text: `مجموعه ${this.$store.state.selectedComplexName}`,
          disabled: true,
        },
      ],
      cards: [
        {
          name: "دستگاه ها",
          action: "devices",
          image: require("@/assets/images/P1000111.jpg"),
        },
        // {
        //   name: "سطوح دسترسی",
        //   action: "permission",
        //   image: require("@/assets/images/security-and-permissions.gif"),
        // },
        {
          name: "اعضای مجموعه",
          action: "members",
          image: require("@/assets/images/208-2089194_cartoon-team-members.png"),
        },
        {
          name: "اعضای باشگاه مشتریان",
          action: "customers",
          image: require("@/assets/images/customer-service-3.jpg"),
        },
      ],
      parentNameFa: "",
      complexParentId: null,
      items: [],
    };
  },

  mounted() {
    this.complexParentId = this.$route.params.id;
    this.getUnits();
  },
  methods: {
    async deleteItem(id) {
      Vue.swal({
        title: "ایا برای حذف این مجموعه مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request.delete(`complex/${id}`).then(() => {
            this.uploadLoading = false;
            Vue.swal("", "مجموعه  با موفقیت حذف گردید", "success");
            this.getComplexes();
          });
        }
      });
    },
    getUnits() {
      request
        .get(`complex/complext-units/${this.complexParentId}`)
        .then((respose) => {
          var data = respose.data.result;
          if (data.length > 0) this.parentNameFa = data[0].parentNameFa;

          this.$store.commit("setSelectedComplexName", this.parentNameFa);
          this.items = data;
        });
    },
    getComplexes() {
      this.getUnits();
    },
    getPermission(action, item) {
      switch (action) {
        case "devices":
          return `ComplexDevice_Show`;
        // case "permission":
        //   return `Permission_List_${item.id}`;
        case "members":
          return `ComplexMember_Show`;
        case "customers":
          return `ComplexCustomerMember_Show`;
        default:
          break;
      }
    },
    getAction(item, action) {
      switch (action) {
        case "devices":
          this.$router.push(`/complex-devices/${item.id}`);
          break;
        case "permission":
          this.$router.push(`/complex-permissions/${item.id}`);
          break;
        case "members":
          this.$router.push(`/complex-members/${item.id}`);
          break;
        case "customers":
          this.$router.push(`/complex-customers/${item.id}`);
          break;
        default:
          break;
      }
    },
    async editItem(item) {
      var parent = this.$refs.addComplexCom;
      parent.dialog = true;
      parent.complexId = item.id;
      parent.isUnit = true;
    },
  },
};
</script>
<style lang="sass" scoped>
.v-card.on-hover.theme--dark
  background-color: rgba(#FFF, 0.8)
  >.v-card__text
    color: #000
</style>
