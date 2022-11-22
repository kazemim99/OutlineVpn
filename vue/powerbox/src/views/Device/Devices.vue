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
    <v-dialog
      v-model="dialog"
      transition="dialog-top-transition"
      max-width="600"
    >
      <v-card>
        <v-card-title class="text-h5"> جزییات </v-card-title>

        <v-card-text>
          <v-radio-group v-model="selectedSubComplexId" mandatory>
            <v-radio
              :value="complexId"
              label="مجموعه اصلی"
              :key="complexId"
            ></v-radio>

            <template v-for="item in subComplexes">
              <v-radio
                :value="item.id"
                :label="item.text"
                :key="item.id"
              ></v-radio>
            </template>
          </v-radio-group>
        </v-card-text>

        <v-card-actions>
          <v-btn color="green darken-1" text @click="addDeviceToComplex()">
            تایید
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn color="green darken-1" text @click="dialog = false">
            بستن
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-data-table
      :headers="headers"
      :items="deviceList"
      :loading="loading"
      :server-items-length="totalDevices"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-if="!isSubComplex" v-slot:item.coplexes="{ item }">
        <v-btn
          v-can="`Devices_ComplexChange`"
          small
          style="margin: -12px"
          color="cyan"
          class="white--text"
          @click="getSubComplex(item.id)"
        >
          تغییر مجموعه
        </v-btn>
      </template>
      <template :disabled="!item.state" v-slot:item.actions="{ item }">
        <v-btn
          v-can="`Devices_Edit_${complexId}`"
          :disabled="!item.state"
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

      <template :disabled="!item.state" v-slot:item.details="{ item }">
        <v-menu offset-y>
          <template v-slot:activator="{ attrs, on }">
            <v-btn
              :disabled="!item.state"
              style="margin: -12px"
              small
              color="primary"
              class="white--text"
              v-bind="attrs"
              v-on="on"
            >
              جزییات
              <v-icon right dark> mdi-arrow-down-drop-circle </v-icon>
            </v-btn>
          </template>

          <v-list>
            <v-list-item
              v-for="menu in items"
              v-can="`${menu.permission}`"
              :key="menu.action"
              link
            >
              <v-list-item-title
                v-text="menu.name"
                @click="getAction(menu.action, item.id, item.name)"
              ></v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
      </template>

      <template v-slot:item.adjust="{ item }">
        <AdjustSetting
          v-if="item.state"
          :selectedComplexId="complexId"
          :currentDevice="item.id"
          ref="adjustSettingRef"
        />
      </template>

      <template v-slot:item.sendSmsStatus="{ item }">
        <label>{{ item.sendSmsStatus ? "فعال" : "غیر فعال" }}</label>
      </template>
      <template v-slot:item.delete="{ item }">
        <v-icon
          v-can="`Devices_Delete_${complexId}`"
          :disabled="!item.state"
          color="red"
          right
          dark
          @click="deleteDevice(item.id)"
        >
          mdi-delete
        </v-icon>
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
      <template v-slot:item.state="{ item }">
        <v-switch
          v-can="'Devices_ChangeState'"
          v-model="item.state"
          flat
          @change="changeDeviceState(item)"
          :label="`${item.state ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>
      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <AddNewDevice
                v-can="`Devices_Create`"
                ref="addDeviceCom"
                @reloadDevices="getDevices"
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
import AddNewDevice from "@/components/device/AddNewDevice.vue";
import AdjustSetting from "@/components/device/AdjustSetting.vue";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "Devices",
  components: {
    AdjustSetting,
    AddNewDevice,
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
          text: "دستگاهها",
          disabled: true,
        },
      ],
      items: [
        {
          action: "deviceState",
          name: "وضعیت فعلی دستگاه",
          permission: "LockerCurrentState_Show",
        },
        {
          action: "deviceCodeActivity",
          name: "کدهای فعالیت",
          permission: "CodeActivity_Show",
        },
        {
          action: "deviceAdvertise",
          name: "محتوی تبلیغاتی",
          permission: "Advertise_Show",
        },
        {
          action: "deviceSmsPanel",
          name: "پنل پیامکی",
          permission: "SMSPanel_Show",
        },
        { action: "devicePin", name: "پین ها", permission: `Pins_Show` },
        {
          action: "deviceReport",
          name: "گزارش استفاده از دستگاه",
          permission: "LockerReport_Show",
        },
      ],
      device: {},
      dialog: false,
      complexId: null,
      totalDevices: 0,
      selectedSubComplexId: 0,
      subComplexes: [],
      pages: 0,
      selectedComplex: 0,
      name: null,
      deviceList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "شناسه", value: "id", sortable: true, align: " d-none" },
        { text: "نام", value: "name", sortable: true },
        { text: "ساعت آغاز کار", value: "startTime", sortable: true },
        { text: "ساعت پایان کار", value: "endTime", sortable: true },
        { text: "تعداد لاکرها", value: "lockerCount", sortable: true },
        { text: "ارسال پیامک", value: "sendSmsStatus", sortable: true },
        { text: "اعتبار پیامک", value: "smsCredit", sortable: true },
        { text: "وضعیت", value: "state", sortable: true },
        { text: "", value: "coplexes", sortable: false, width: "1%" },
        { text: "", value: "details", sortable: false, width: "1%" },
        { text: "", value: "actions", sortable: false, width: "1%" },
        { text: "", value: "adjust", sortable: false, width: "1%" },
        { text: "", value: "delete", sortable: false, width: "1%" },
      ],
    };
  },

  watch: {
    options: {
      handler() {
        this.getDevices();
      },
      deep: true,
    },
    name: function () {
      if (this.name.length > 2 || this.name.length === 0) {
        this.options.page = 1;
        this.options.name = this.name;
        this.getDevices();
      }
    },
  },
  mounted() {
    this.complexId = this.$route.params.id;
    this.$store.commit("setSelectedComplexId", this.complexId);
    this.getDevices();
    this.getIsSubComplex();
  },

  methods: {
    async changeDeviceState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/device/change-state/${item.id}`)
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

    async editItem(item) {
      this.$refs.addDeviceCom.dialog = true;
      this.$refs.addDeviceCom.deviceId = item.id;
    },

    deleteDevice(deviceId) {
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
          request.delete(`/device/${deviceId}`).then(() => {
            Vue.swal("", "دستگاه با موفقیت حذف گردید", "success");
            this.getDevices();
          });
        }
      });
    },
    next(page) {
      this.options.page = page;
      this.getDevices();
    },
    handler(event) {
      this.options = event;
    },
    getAction(action, deviceId, name) {
      this.$store.commit("setSelectedDeviceName", name);
      switch (action) {
        case "deviceState":
          this.$router.push(`/device-state/${deviceId}`);
          break;

        case "deviceCodeActivity":
          this.$router.push(`/device-code-activity/${deviceId}`);
          break;

        case "deviceAdvertise":
          this.$router.push(`/device-advertise/${deviceId}`);
          break;
        case "deviceSmsPanel":
          this.$router.push(`/device-sms-panel/${deviceId}`);
          break;
        case "devicePin":
          this.$router.push(`/device-pins/${deviceId}`);
          break;
        case "deviceReport":
          this.$router.push(`/device-report/${deviceId}`);
          break;

        default:
          break;
      }
    },
    getSubComplex(deviceId) {
      request
        .get(`/publicData/sub-complexes/${this.complexId}`)
        .then((response) => {
          var data = response.data.result;
          this.dialog = true;
          console.log(data);
          this.subComplexes = data;
          this.selectedDevice = deviceId;
        });
    },
    getIsSubComplex() {
      request
        .get(`/publicData/is-subComplex/${this.complexId}`)
        .then((response) => {
          var data = response.data.result;
          this.isSubComplex = data;
        });
    },

    addDeviceToComplex() {
      request
        .put(
          `/device/changeDeviceComplex/${this.selectedSubComplexId}/${this.selectedDevice}`
        )
        .then(() => {
          this.dialog = false;
        });
    },
    async getDevices() {
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
        .get(`/device/devices/${this.complexId}?` + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.deviceList = data.result;
          this.totalDevices = data.totalItems;
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
