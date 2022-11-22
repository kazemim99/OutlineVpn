<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="securityImageList"
      :loading="loading"
      :server-items-length="totalSecurityImages"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.image="{ item }">
        <img
          @click="getModal(item.image)"
          :src="item.image"
          width="50"
          height="50"
        />
      </template>
    </v-data-table>
    <v-pagination
      v-model="options.page"
      @input="next"
      :length="this.pages"
      :total-visible="7"
    ></v-pagination>
    <rotate ref="modal"></rotate>
  </div>
</template>
<script>
import request from "@/utils/request";
import Breadcrump from "@/components/common/Breadcrump.vue";
import rotate from "@/components/common/Rotate.vue";

export default {
  name: "SecurityImages",
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
          text: "تصاویر امنیتی",
          disabled: true,
        },
      ],
      securityImage: {},
      totalSecurityImages: 0,
      pages: 0,
      name: null,
      address: null,
      securityImageList: [],
      loading: true,
      options: {
        mustSort: true,
        sortDesc: [false],
      },

      headers: [
        { text: "مجموعه", value: "complextName", sortable: false },
        { text: "دستگاه ", value: "deviceName", sortable: false },
        { text: "لاکر ", value: "lockerNumber", sortable: false },
        { text: "تصویر", value: "image", sortable: false },
        { text: "تاریخ", value: "date", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getSecurityImages();
      },
      deep: true,
    },
  },
  mounted() {
    this.getSecurityImages();
  },

  methods: {
    getModal(img) {
      this.$refs.modal.openModel(img);
    },
    next(page) {
      this.options.page = page;
      this.getSecurityImages();
    },
    handler(event) {
      this.options = event;
    },
    clearFrom() {
      this.options.from = null;
    },
    clearTo() {
      this.options.to = null;
    },
    async getSecurityImages() {
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
        .get("/support/security-images?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.securityImageList = data.result;
          this.totalSecurityImages = data.totalItems;
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
