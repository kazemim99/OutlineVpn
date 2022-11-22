<template>
  <div>
    <Breadcrump :crumbs="crumbs" />
    <v-data-table
      :headers="headers"
      :items="complexList"
      :loading="loading"
      :server-items-length="totalComplexes"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.actions="{ item }">
        <v-btn
          v-can="`Complex_Edit_${item.id}`"
          :disabled="!item.state"
          color="cyan"
          class="ma-2 white--text"
          @click="editItem(item)"
        >
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>

           <v-btn
          v-can="`Complex_DeleteComplex_${item.id}`"
          :disabled="!item.state"
          color="error"
          class="ma-2 white--text"
          @click="deleteComplex(item.id)"
        >
          حذف
          <v-icon right dark> mdi-trash-can </v-icon>
        </v-btn>
      </template>
   
      <template v-slot:item.childaction="{ item }">
        <v-btn
          v-can="`Complex_DetailsShow`"
          :disabled="!item.state"
          color="primary"
          class="ma-2 white--text"
          @click="complexUnits(item.id)"
        >
          مشاهده
          <v-icon right dark> mdi-eye </v-icon>
        </v-btn>
      </template>

      <template v-slot:header.nameFa="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="nameFa ? 'primary' : ''"
                >mdi-magnify</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="nameFa"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="nameFa = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>
      <template v-slot:item.state="{ item }">
        <v-switch
          v-can="`Complex_Active_${item.id}`"
          v-model="item.state"
          flat
          @change="changeComplexState(item)"
          :label="`${item.state ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewComplex
                v-can="`Complex_Create`"
                ref="addComplexCom"
                @reloadComplexes="getComplexes"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست مجموعه ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:header.name="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="firstName ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="firstName"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="firstName = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>
    </v-data-table>
    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNewComplex from "@/components/complex/AddNewComplex.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "Complexes",
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
          disabled: true,
          href: "/complexes",
        },
      ],
      complex: {},
      totalComplexes: 0,
      pages: 0,
      nameFa: null,
      address: null,
      complexList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "شناسه", value: "id", sortable: false },
        { text: "نام", value: "nameFa", sortable: true },
        { text: "آدرس", value: "address", sortable: true },
        { text: "وضعیت", value: "state", sortable: true, width: "20%" },
        { text: "", value: "actions", sortable: false, width: "20%" },
        { text: "", value: "childaction", sortable: false, width: "10%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getComplexes();
      },
      deep: true,
    },
    nameFa: function () {
      if (this.nameFa.length > 2 || this.nameFa.length === 0) {
        this.options.page = 1;
        this.options.nameFa = this.nameFa;
        this.getComplexes();
      }
    },
  },
  mounted() {
    this.getComplexes();
  },

  methods: {
    async deleteComplex(id) {
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
          request.delete(`/complex/${id}`).then(() => {
            this.uploadLoading = false;
            Vue.swal("", "مجموعه  با موفقیت حذف گردید", "success");
            this.getComplexes();
          });
        }
      });
    },
    async changeComplexState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/complex/change-state/${item.id}`)
        .then((response) => {
          console.log(item.state);
        })
        .catch((error) => {
          this.userState = !this.state;
        })
        .finally(() => {
          this.loading = false;
        });
    },

    async complexUnits(id) {
      this.$router.push(`/complex-units/${id}`);
    },
    async editItem(item) {
      this.$refs.addComplexCom.dialog = true;
      this.$refs.addComplexCom.complexId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getComplexes();
    },
    handler(event) {
      this.options = event;
    },
    async getComplexes() {
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      this.loading = true;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get("/complex/complexes?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.complexList = data.result;
          this.totalComplexes = data.totalItems;
          this.pages = data.pageCount;
        })
        .catch((error) => {
          alert(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>
