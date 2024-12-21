<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="dnsRecordList"
      :loading="loading"
      :server-items-length="totalDNSRecords"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.enable="{ item }">
        <v-switch
          v-model="item.enable"
          flat
          @change="changeState(item)"
          :label="`${item.enable ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.isActive="{ item }">
        <v-switch
          v-model="item.isActive"
          flat
          @change="changeActive(item)"
          :label="`${item.isActive ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.hasLoadBalance="{ item }">
        <v-switch
          v-model="item.hasLoadBalance"
          flat
          @change="changeLoadBalance(item)"
          :label="`${item.hasLoadBalance ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>

      <template v-slot:item.adjust="{ item }">
        <v-btn
          :loading="loadingItems[item.id]"
          color="blue darken-1"
          @click="adjustItem(item)"
          >تنظیم</v-btn
        >
      </template>

      <template v-slot:item.edit="{ item }">
        <v-icon
          v-can="'Member_Edit'"
          medium
          class="mr-2"
          @click="editItem(item)"
          >mdi-pencil</v-icon
        >
      </template>

      <template v-slot:item.delete="{ item }">
        <v-icon
          v-can="'Member_Delete'"
          medium
          class="mr-2"
          @click="deleteItem(item.id)"
          >mdi-delete</v-icon
        >
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewDNSRecord
                ref="addDNSRecordCom"
                @reloadDNSRecords="getDNSRecords"
              />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست سرور ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:header.title="{ header }">
        {{ header.text }}
        <v-menu offset-y left :close-on-content-click="false">
          <template v-slot:activator="{ on, attrs }">
            <v-btn icon v-bind="attrs" v-on="on">
              <v-icon small :color="title ? 'primary' : ''">mdi-filter</v-icon>
            </v-btn>
          </template>
          <div style="background-color: white; width: 280px">
            <v-text-field
              v-model="title"
              class="pa-4"
              type="text"
              label="جستجو"
            ></v-text-field>
            <v-btn
              @click="title = ''"
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
import AddNewDNSRecord from "@/components/DNSRecords/AddNewDNSRecord.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";
import AezaAPI from "aeza-net-sdk";

export default {
  name: "DNSRecords",
  components: {
    // states
    AddNewDNSRecord,
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
          text: "DNS",
          disabled: true,
        },
      ],
      DNSRecord: {},
      loadingItems: {},
      totalDNSRecords: 0,
      switchLoading: null,
      pages: 0,
      hasLicense: null,
      isActive: null,
      hasLoadBalance: null,
      title: null,
      dnsRecordList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },
      headers: [
        { text: "IPV4", value: "ipv4", sortable: true },
        { text: "IPV6", value: "ipv6", sortable: true },
        { text: "ویرایش", value: "edit", sortable: false },
        { text: "حذف", value: "delete", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getnsRecords();
      },
      deep: true,
    },
    title: function () {
      if (this.title.length > 2 || this.title.length === 0)
        this.options.page = 1;
      this.options.title = this.title;

      this.getDNSRecords();
    },
  },
  mounted() {
    this.getDNSRecords();
  },

  methods: {
    async adjustItem(item) {
      this.$set(this.loadingItems, item.id, true);
      this.loading = true;
      await request
        .put(`/sshkey/adjust/${item.id}`)
        .then(() => {
          this.loading = false;
        })
        .catch((error) => {
          this.loading = false;
        })
        .finally(() => {
          this.loading = false;
          this.$set(this.loadingItems, item.id, false);
        });
    },
    async changeState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/dnsRecord/change-state/${item.id}`)
        .then(() => {
          this.getDNSRecords();
        })
        .catch((error) => {
          alert(error);
          this.hasLicense = !this.hasLicense;
        })
        .finally(() => {
          this.$set(this.loadingItems, item.id, false);
        });
    },

    async changeActive(item) {
      debugger;
      
      const api = new AezaAPI('ea09de9d2f1a7952ae8e524074a8ec92');
      const { response } = await api.profile.get();

      console.log(response);
      
      this.switchLoading = "warning";
      await request
        .put(`/dnsRecord/change-active/${item.id}`)
        .then(() => {
          this.getDNSRecords();
        })
        .catch((error) => {
          alert(error);
          this.isActive = !this.isActive;
        })
        .finally(() => {
          this.$set(this.loadingItems, item.id, false);
        });
    },

    async changeLoadBalance(item) {
      this.switchLoading = "warning";
      await request
        .put(`/dnsRecord/change-loadBalance/${item.id}`)
        .then(() => {
          this.getDNSRecords();
        })
        .catch((error) => {
          alert(error);
          this.hasLoadBalance = !this.hasLoadBalance;
        })
        .finally(() => {
          this.$set(this.loadingItems, item.id, false);
        });
    },
    async editItem(item) {
      this.$refs.addDNSRecordCom.dialog = true;
      this.$refs.addDNSRecordCom.id = item.id;
    },
    deleteItem(id) {
      Vue.swal({
        title: "ایا مطمئن  هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,حذف شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .delete(`/DNSRecord/${id}`)
            .then(() => {
              Vue.swal("", "سرور با موفقیت حذف گردید", "success");
              this.getDNSRecords();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

    next(page) {
      this.options.page = page;
      this.getDNSRecords();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.state = state;
    },

    async getDNSRecords() {
      const { sortBy, sortDesc, page, itemsPerPage } = this.options;
      this.loading = true;

      const filterQuery = Object.keys(this.options)
        .filter(
          (x) => this.options[x] !== null && this.options[x] !== undefined
        )
        .map((key) => `${key}=${this.options[key]}`)
        .join("&");

      this.loading = true;
      await request
        .get("/dnsRecord/filter?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.dnsRecordList = data.result;
          this.totalDNSRecords = data.totalItems;
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
