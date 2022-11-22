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
      :items="customerList"
      :loading="loading"
      :server-items-length="totalCustomeres"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.actions="{ item }">
        <v-btn
          v-can="`CustomerMember_Edit_${complexId}`"
          color="cyan"
          class="ma-2 white--text"
          @click="editItem(item)"
        >
          ویرایش
          <v-icon right dark> mdi-pencil </v-icon>
        </v-btn>
      </template>

      <template v-slot:header.name="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="name ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="name"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="name = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>

      <template v-slot:header.phoneNumber="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="phoneNumber ? 'primary' : ''"
                >mdi-filter</v-icon
              >
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="phoneNumber"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="phoneNumber = ''"
              small
              text
              color="primary"
              class="ml-2 mb-2"
              >پاک کردن</v-btn
            >
          </div>
        </v-menu>
      </template>
      <template v-slot:item.delete="{ item }">
        <v-icon
          v-can="`CustomerMember_Delete_${complexId}`"
          color="red"
          right
          dark
          @click="deleteCustomer(item)"
        >
          mdi-delete
        </v-icon>
      </template>
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewCustomer
                v-can="`CustomerMember_Add_${complexId}`"
                ref="addCustomerCom"
                @reloadCustomeres="getCustomeres"
              />
            </template>
          </v-col>
          <v-col cols="2">
            <v-btn
              v-can="'CustomerMember_Excel'"
              :loading="uploadLoading"
              :disabled="uploadLoading"
              color="blue-grey"
              class="ma-2 white--text"
              @click="openFile"
            >
              فایل اکسل
              <v-icon dark style="margin-right: 4px"> mdi-cloud-upload </v-icon>
            </v-btn>
            <v-file-input
              ref="uploadbtn"
              hidden
              hide-details=""
              hide-input
              v-model="excelFile"
              type="file"
              accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
              truncate-length="1"
              @change="onFileChange"
            />
          </v-col>

          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست باشگاه مشتریان</v-toolbar-title>
        </v-toolbar>
      </template>
    </v-data-table>
    <v-pagination
      v-can="'CustomerMember_List'"
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
  </div>
</template>
<script>
import request from "@/utils/request";
import AddNewCustomer from "@/components/complex/AddNewCustomer.vue";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "ComplexCustomers",
  components: {
    AddNewCustomer,
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
          text: `مجموعه  ${this.$store.state.selectedComplexName}`,
          disabled: false,
          href: `/complex-units/${this.$route.params.id}`,
        },
        {
          text: "باشگاه مشتریان ",
          disabled: true,
        },
      ],
      excelFile: null,
      complexId: null,
      uploadLoading: false,
      customer: {},
      totalCustomeres: 0,
      pages: 0,
      customerId: 0,
      dialog: false,
      name: null,
      phoneNumber: null,
      customerList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "نام", value: "name", sortable: true },
        { text: "تلفن", value: "phoneNumber", sortable: true },
        { text: "", value: "actions", sortable: false, width: "10%" },
        { text: "", value: "delete", sortable: false, width: "5%" },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getCustomeres();
      },
      deep: true,
    },
    name: function () {
      if (this.name.length > 2 || this.name.length === 0) {
        this.options.page = 1;
        this.options.name = this.name;
        this.getCustomeres();
      }
    },
    phoneNumber: function () {
      if (this.phoneNumber.length > 2 || this.phoneNumber.length === 0) {
        this.options.page = 1;
        this.options.phoneNumber = this.phoneNumber;
        this.getCustomeres();
      }
    },
  },
  mounted() {
    this.getCustomeres();
    this.complexId = this.$route.params.id;
  },

  methods: {
    openFile() {
      this.$refs.uploadbtn.$refs.input.click();
    },
    onFileChange(file) {
      if (!file) {
        return;
      }
      Vue.swal({
        title: "ایا برای ارسال این فایل مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,ارسال شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          this.uploadLoading = true;
          var form_data = new FormData();

          form_data.append("file", this.excelFile);

          request.defaults.headers.common.accept = "multipart/form-data";
          request
            .put(
              `/customerClub/upload-file/${this.$route.params.id}`,
              form_data
            )
            .then(() => {
              Vue.swal("", "نقش  با موفقیت بارگزاری گردید", "success");
              this.getCustomeres();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    deleteCustomer(item) {
      Vue.swal({
        title: "ایا برای حذف این مشتری مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request.delete(`/customerClub/${item.id}`).then(() => {
            this.uploadLoading = false;
            Vue.swal("", "نقش  با موفقیت حذف گردید", "success");
            this.getCustomeres();
          });
        }
      });
    },
    async editItem(item) {
      this.$refs.addCustomerCom.dialog = true;
      this.$refs.addCustomerCom.customerId = item.id;

      this.dialog = true;
      this.customerId = item.id;
    },

    next(page) {
      this.options.page = page;
      this.getCustomeres();
    },
    handler(event) {
      this.options = event;
    },
    async getCustomeres() {
      const { sortDesc, sortBy, page, itemsPerPage } = this.options;
      this.loading = true;
      this.options.complexId = this.$route.params.id;
      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get(`/customerClub/customers/${this.$route.params.id}?` + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.customerList = data.result;
          this.totalCustomeres = data.totalItems;
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
<style scoped>
.v-input {
  display: none;
}
</style>
