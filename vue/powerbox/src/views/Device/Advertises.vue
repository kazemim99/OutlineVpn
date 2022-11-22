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

    <v-data-table
      :headers="headers"
      :items="advertiseList"
      :loading="loading"
      :server-items-length="totalAdvertises"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template :disabled="!item.state" v-slot:item.edit="{ item }">
        <v-btn
          v-can="`Advertise_Edit_${deviceId}`"
          small
          style="margin: -12px"
          color="cyan"
          class="white--text"
          @click="editItem(item)"
        >
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>
      </template>
    
      <template v-slot:item.delete="{ item }">
        <v-icon
          v-can="`Advertises_Delete_${deviceId}`"
          color="red"
          right
          dark
          @click="deleteAdvertise(item.id)"
        >
          mdi-delete
        </v-icon>
      </template>
  
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewAdvertise
                v-can="`Advertises_Create_${deviceId}`"
                ref="addAdvertiseCom"
                @reloadAdvertises="getAdvertises"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست تبلیغات </v-toolbar-title>
        </v-toolbar>
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
import AddNewAdvertise from "@/components/advertise/AddNewAdvertise.vue";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "Advertises",
  components: {
    AddNewAdvertise,
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
          disabled: false,
          href: `/complex-units/${this.$route.params.id}`,
        },
        {
          text: "تبلیغ ها",
          disabled: true,
        },
      ],
      advertise: {},
      deviceId: null,
      totalAdvertises: 0,
      pages: 0,
      advertiseList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام", value: "name", sortable: true },
        { text: "ساعت آغاز کار", value: "startTime", sortable: true },
        { text: "ساعت پایان کار", value: "endTime", sortable: true },
        { text: "ترتیب", value: "periority", sortable: true },
        { text: "مدت زمان توقف", value: "stopImageTimeSecond", sortable: true },
        { text: "", value: "edit", sortable: false, width: "1%" },
        { text: "", value: "delete", sortable: false, width: "1%" },
      ],
    };
  },

  watch: {
    options: {
      handler() {
        this.getAdvertises();
      },
      deep: true,
    },
    name: function () {
      if (this.name.length > 2 || this.name.length === 0) {
        this.options.page = 1;
        this.options.name = this.name;
        this.getAdvertises();
      }
    },
  },
  mounted() {
    this.deviceId = this.$route.params.id;
    this.getAdvertises();
  },

  methods: {
 
    async editItem(item) {
      this.$refs.addAdvertiseCom.dialog = true;
      this.$refs.addAdvertiseCom.advertiseId = item.id;
    },
   
    deleteAdvertise(advertiseId) {
      Vue.swal({
        title: "برای حذف این دستگاه مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request.delete(`/advertise/${advertiseId}`).then(() => {
            Vue.swal("", "تبلیغ با موفقیت حذف گردید", "success");
            this.getAdvertises();
          });
        }
      });
    },
    next(page) {
      this.options.page = page;
      this.getAdvertises();
    },
    handler(event) {
      this.options = event;
    },
 
    async getAdvertises() {
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
        .get(`/advertise/advertises/${this.deviceId}?` + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.advertiseList = data.result;
          this.totalAdvertises = data.totalItems;
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
