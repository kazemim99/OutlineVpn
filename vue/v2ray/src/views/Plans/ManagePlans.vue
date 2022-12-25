<template>
  <div>
    <Breadcrump :crumbs="crumbs" />

    <v-data-table
      :headers="headers"
      :items="planList"
      :loading="loading"
      :server-items-length="totalPlans"
      item-key="id"
      :options.sync="options"
      class="elevation-1"
    >
      <template v-slot:item.planState="{ item }">
        <v-switch
          v-model="item.planState"
          flat
          @change="changePlanState(item)"
          :label="`${item.planState ? 'فعال' : 'غیر فعال'}`"
        ></v-switch>
      </template>
 

      <template  v-slot:item.edit="{ item }">
        <v-icon v-can="'Member_Edit'" medium class="mr-2" @click="editItem(item)">mdi-pencil</v-icon>
      </template>

    

         <template  v-slot:item.delete="{ item }">
        <v-icon v-can="'Member_Delete'" medium class="mr-2" @click="deleteItem(item.id)">mdi-delete</v-icon>
      </template>

      <template v-slot:top>
        <v-toolbar flat>
          <v-col cols="3">
            <template right >
              <AddNewPlan v-can="'Member_Create'" ref="addPlanCom" @reloadPlans="getPlans" />
            </template>
          </v-col>
          <v-spacer></v-spacer>
          <v-divider class="mx-4" inset vertical></v-divider>

          <v-toolbar-title>لیست پلن ها</v-toolbar-title>
        </v-toolbar>
      </template>
      <template v-slot:header.title="{ header }">
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
import AddNewPlan from "@/components/plans/AddNewPlan.vue";
import Breadcrump from "@/components/common/Breadcrump.vue";
import Vue from "vue";

export default {
  name: "Plans",
  components: {
    // PlanStates
    AddNewPlan,
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
          text: "پلن",
          disabled: true,
        },
      ],
      plan: {},
      totalPlans: 0,
      switchLoading: null,
      pages: 0,
      planState: null,
      title: null,
      lastName: null,
      mobile: null,
      planList: [],
      loading: true,
      options: { mustSort: true, sortDesc: [false] },

      headers: [
        { text: "عنوان", value: "title", sortable: true },
        { text: "قیمت", value: "price", sortable: true },
        { text: "ظرفیت", value: "trafficCapacity", sortable: true },
        { text: "مدت", value: "priod", sortable: false },
        { text: "وضعیت", value: "planState", sortable: false },
        { text: "", value: "edit", sortable: false },
        { text: "", value: "delete", sortable: false },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.getPlans();
      },
      deep: true,
    },
    title: function () {
      if (this.title.length > 2 || this.title.length === 0)
        this.options.page = 1;
      this.options.title = this.title;

      this.getPlans();
    },
  },
  mounted() {
    this.getPlans();
  },

  methods: {
    async changePlanState(item) {
      this.switchLoading = "warning";
      await request
        .put(`/plan/change-state/${item.id}`)
        .then(() => {
          console.log(item.planState);
        })
        .catch((error) => {
          alert(error);
          this.stae = !this.planState;
        })
        .finally(() => {
          this.loading = false;
        });
    },
    async editItem(item) {
      this.$refs.addPlanCom.dialog = true;
      this.$refs.addPlanCom.id = item.id;
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
            .delete(`/plan/${id}`)
            .then(() => {
              Vue.swal("", "پلن با موفقیت حذف گردید", "success");
              this.getPlans();
            })
            .finally(() => {
              this.uploadLoading = false;
            });
        }
      });
    },

  
    next(page) {
      this.options.page = page;
      this.getPlans();
    },
    handler(event) {
      this.options = event;
    },
    GetSelectedState(state) {
      this.planState = state;
    },

    async getPlans() {
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
        .get("/plan/plans?" + filterQuery)
        .then((response) => {
          var data = response.data.result;
          this.planList = data.result;
          this.totalPlans = data.totalItems;
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
