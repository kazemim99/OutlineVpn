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
      <v-col>
      <label cols="3"
        >حافظه خالی : <span>{{ this.freeMemory }}</span></label
      >
      </v-col>
      <v-col>
      <label  cols="3"
        >حافظه کل : <span>{{ this.totalMemory }}</span></label
      >
      </v-col>
    </v-row>
    <br/>
    <br/>
    <v-data-table
      :headers="headers"
      :items="lockers"
      :loading="loading"
      item-key="id"
      class="elevation-1"
    >
      <template v-slot:item.active="{ item }">
        <v-switch
          v-can="'LockerChange_Active'"
          v-model="item.active"
          flat
          @change="changeLockerActivation(item)"
          :label="`${item.active ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>
      <template v-slot:item.receiveImageData="{ item }">
        <img
          @click="getModal(item.receiveImageData)"
          :src="item.receiveImageData"
          width="50"
          height="50"
        />
      </template>
      <template v-slot:item.chargeImageData="{ item }">
        <img
          @click="getModal(item.chargeImageData)"
          :src="item.chargeImageData"
          width="50"
          height="50"
        />
      </template>

      <template v-slot:item.lockImageData="{ item }">
        <img
          @click="getModal(item.lockImageData)"
          :src="item.lockImageData"
          width="50"
          height="50"
        />
      </template>
      <template v-slot:item.permanentImageData="{ item }">
        <img
          @click="getModal(item.permanentImageData)"
          :src="item.permanentImageData"
          width="50"
          height="50"
        />
      </template>
      <template v-slot:item.openLocker="{ item }">
        <v-btn
          :disabled="!item.active"
          small
          v-can="'LockerOpen_Online'"
          color="cyan"
          
          class="white--text ml-5"
          @click="openLocker(item)"
        >
          بازکردن لاکر
          <v-icon right dark> mdi-locker </v-icon>
        </v-btn>


         <v-btn
          :disabled="!item.active"
          small
          v-can="'EmptyLocker_Online'"
          color="error"
          class="white--text"
          @click="emptyLocker(item)"
        >
          خالی کردن لاکر 
          <v-icon right dark> mdi-bookmark-remove </v-icon>
        </v-btn>
      </template>
    </v-data-table>
    <rotate ref="modal"></rotate>
  </div>
</template>
<script>
import request from "@/utils/request";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import rotate from "@/components/common/Rotate.vue";

export default {
  name: "DeviceState",
  components: {
    Breadcrump,
    rotate,
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
          href: `/complex-devices/${this.$store.state.selectedComplexId}`,
        },
        {
          text: `${this.$store.state.selectedDeviceName}`,
          disabled: true,
        },
      ],
      name: null,
      lockers: [],
      freeMemory: 0,
      totalMemory: 0,
      loading: true,
      headers: [
        { text: "شماره لاکر", value: "lockerNo", sortable: false },
        { text: "وضعیت", value: "state", sortable: false },
        { text: "وضعیت درب", value: "doorOpenCloseState", sortable: false },
        { text: " شماره تلفن", value: "phoneNumber", sortable: false },
        { text: "زمان آغاز", value: "startDateTime", sortable: false },
        {
          text: "عکس امنیتی",
          value: "securityImage",
          sortable: false,
        },
        {
          text: "عکس کاربر شارژ",
          value: "chargeImageData",
          sortable: false,
        },
        { text: "وضعیت", value: "active", sortable: false },
        { text: "", value: "openLocker", sortable: false, width: "20%" },
      ],
    };
  },
  created() {
    this.deviceId = this.$route.params.id;
    this.getLockers();
  },
  methods: {
    getModal(img) {
      this.$refs.modal.openModel(img);
    },
    async changeLockerActivation(item) {
      this.switchLoading = "warning";
      await request
        .put(
          `/deviceManagement/change-active/${this.deviceId}/${item.lockerNo}`
        )
        .finally(() => {
          this.getLockers();
          this.loading = false;
        });
    },
    openLocker(item) {
      Vue.swal({
        title: "برای باز کردن این لاکر مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,باز شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .put(
              `/deviceManagement/open-locker/${this.deviceId}/${item.lockerNo}`
            )
            .then(() => {
              this.getLockers();

              Vue.swal("", "درخواست با موفقیت ارسال گردید", "success");
            });
        }
      });
    },
    emptyLocker(item) {
      Vue.swal({
        title: "برای خالی کردن این لاکر مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,خالی  شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .put(
              `/deviceManagement/empty-locker/${this.deviceId}/${item.lockerNo}`
            )
            .then(() => {
              this.getLockers();

              Vue.swal("", "درخواست با موفقیت ارسال گردید", "success");
            });
        }
      });
    },

    async getLockers() {
      this.loading = true;
      await request
        .get(`/deviceManagement/lockers/${this.deviceId}`)
        .then((response) => {
          var data = response.data.result;
          this.totalMemory = data[0].totalMemory;
          this.freeMemory = data[0].freeMemory;
          this.lockers = data;
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
