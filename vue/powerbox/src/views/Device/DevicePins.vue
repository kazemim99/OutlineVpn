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
      :items="pins"
      :loading="loading"
      item-key="id"
      class="elevation-1"
    >
      <template v-slot:item.isActive="{ item }">
        <v-chip  class="ma-2" :color="`${!item.isActive ? 'primary' : 'green'}`">
          {{ !item.isActive ? "غیر فعال" : "فعال" }}
        </v-chip>
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right>
              <v-btn v-can="`Pins_Regenerate_${complextId}`" color="primary" @click="RegeneratePin" dark
                >بازنشانی پین ها
              </v-btn>
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست پین ها</v-toolbar-title>
        </v-toolbar>
      </template>

      <template v-slot:item.edit="{ item }">
        <div class="text-center">
          <v-btn v-can="`Pins_Edit_${complextId}`" small color="cyan" @click="openDialog(item.id)" dark>
            ویرایش
            <v-icon right dark> mdi-pencil </v-icon>
          </v-btn>
        </div>
      </template>
    </v-data-table>
    <v-dialog v-model="dialog" width="500">
      <v-card>
        <v-card-title class="text-h5 grey lighten-2"> ویرایش پین </v-card-title>

        <v-col cols="12" sm="12" md="12">
          <v-text-field
            outlined
            clearable
            v-model="selectedPin.pin"
            label="پین  *"
            placeholder=" "
            autocomplete="false"
            required
          ></v-text-field>
        </v-col>
        <v-col cols="12" sm="12" md="12">
          <v-text-field
            outlined
            clearable
            v-model="selectedPin.imageNumber"
            label="تصویر  *"
            placeholder=" "
            autocomplete="false"
            required
          ></v-text-field>
        </v-col>
        <v-divider></v-divider>

        <v-card-actions>
          <v-spacer></v-spacer>

          <v-btn color="green darken-1" text @click="dialog = false">
            انصراف
          </v-btn>
          <v-btn v-can="'Pins_Edit'" @click="editItem()" color="green darken-1" text> ثبت </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>
<script>
import request from "@/utils/request";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "Pins",
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
          href: `/complex-units/${this.$route.params.id}`,
        },
        {
          text: "دستگاهها",
          href: `/complex-devices/${this.$store.state.selectedComplexId}`,
        },
        {
          text: `پین ها${this.$store.state.selectedDeviceName}`,
          disabled: true,
        },
      ],
      name: null,
      dialog: false,
      selectedPin: {},
      complextId:null,
      pins: [],
      loading: true,
      headers: [
        { text: "", value: "id", sortable: false, align: " d-none" },
        { text: "", value: "deviceId", sortable: false, align: " d-none" },
        { text: "پین", value: "pin", sortable: false },
        { text: "وضعیت", value: "isActive", sortable: false },
        { text: "تصویر", value: "imageNumber", sortable: false },
        { text: "", value: "edit", sortable: false },
      ],
    };
  },

  created() {
    this.deviceId = this.$route.params.id;
    this.complextId = this.$store.state.selectedComplexId;
    this.getPins();
  },
  methods: {
    openDialog(id) {
      this.dialog = true;
      this.selectedPinId = id;
      this.selectedPin = Object.assign(
        {},
        this.pins.filter((a) => a.id == id)[0]
      );
    },
    async editItem() {
      request
        .put(
          `/deviceManagement/edit-master-pin/${this.selectedPin.id}`,
          this.selectedPin
        )
        .then(() => {
          this.dialog = false;
          Vue.swal("", "پین  با موفقیت ویرایش شد", "success");
          this.getPins();
        });
    },
    async RegeneratePin() {
      Vue.swal({
        title: "برای باز نشانی پینها مطمئن هستید",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "بله ,بازنشانی شود",
        cancelButtonText: "انصراف",
      }).then((result) => {
        if (result.isConfirmed) {
          request
            .put(`/deviceManagement/re-generate-master-pin/${this.deviceId}`)
            .then(() => {
              Vue.swal("", "پین ها با موفقیت بازنشانی شدند", "success");
              this.getPins();
            });
        }
      });
    },
    async getPins() {
      this.loading = true;
      await request
        .get(`/deviceManagement/pins/${this.deviceId}`)
        .then((response) => {
          var data = response.data.result;
          this.pins = data;
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
