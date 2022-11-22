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
      <v-col cols="10">
        <v-textarea
          v-model="content"
          auto-grow
          label="متن"
          outlined
          rows="2"
          row-height="25"
          shaped
        ></v-textarea>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="10">
        <v-textarea
          label="متن باشگاه مشتریان "
          auto-grow
          v-model="customerClubSmsContent"
          outlined
          rows="2"
          row-height="25"
          shaped
        ></v-textarea>
      </v-col>
    </v-row>
    <v-btn color="blue darken-1" v-can="`SMSPanel_Edit_${selectedComplexId}`" @click="submit()"
      >ذخیره</v-btn
    >
  </div>
</template>

<script>
import request from "@/utils/request";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "DeviceSmsPanel",
  components: {
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
          href: `/complex-units/${this.selectedComplexId}`,
        },
        {
          text: `${this.$store.state.selectedDeviceName}`,
          href: `/complex-devices/${this.$route.params.id}`,
          disabled: false,
        },
        {
          text: "پنل پیامک ",
          disabled: true,
        },
      ],
      selectedComplexId:null,
      content: null,
      customerClubSmsContent: null,
      deviceId: 0,
    };
  },
  created() {
    this.deviceId = parseInt(this.$route.params.id);
    this.selectedComplexId=this.$store.state.selectedComplexId
    this.getSmsContent();
  },
  methods: {
    getSmsContent() {
      request.get(`/device/sms-content/${this.deviceId}`).then((response) => {
        var data = response.data.result;
        this.content = data.content;
        (this.customerClubSmsContent = data.customerClubSmsContent),
          (this.id = data.id);
      });
    },
    submit() {
      request
        .put(`/device/sms-content/${this.deviceId}`, {
          content: this.content,
          customerClubSmsContent: this.customerClubSmsContent,
        })
        .then(() => {
          Vue.swal("", "اطلاعات با موفقیت ذخیره گردید", "success");
        });
    },
  },
};
</script>

<style>
</style>