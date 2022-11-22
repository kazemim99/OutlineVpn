<template>
  <div class="text-center">
    <v-dialog v-model="dialog" height="500px">
      <template v-slot:activator="{ on, attrs }">
        <v-btn color="red lighten-2" dark v-bind="attrs" v-on="on">
          مجموعه ها
        </v-btn>
      </template>

      <template>
        <v-card>
          <v-card-title class="indigo white--text text-h5">
            مجموعه ها
          </v-card-title>
          <v-row class="pa-4" justify="space-between">
            <v-col cols="5">
              <v-treeview
                :active.sync="active"
                :items="complexes"
                activatable
                color="warning"
                @update:active="getRoles"
                open-on-click
                transition
              >
                <template v-slot:prepend="{ item }">
                  <v-icon v-if="!item.children"> mdi-office-building </v-icon>
                  <v-text>{{ item.nameFa }}</v-text>
                </template>
              </v-treeview>
            </v-col>

            <v-divider vertical></v-divider>

            <v-col cols="7" class="d-flex text-right">
              <v-scroll-y-transition mode="out-in">
                <div
                  v-if="!complexRoles"
                  class="text-h6 grey--text text--lighten-1 font-weight-light"
                  style="align-self: end"
                >
                  نقشهای مجموعه
                </div>
                <v-card v-else class="pt-6" flat>
                  <v-row>
                    <v-list>
                      <v-list-item v-for="role in complexRoles" :key="role.id">
                        <v-col cols="3">
                          <v-checkbox
                            :label="role.title"
                            v-model="selectedRoles"
                            :value="role.id"
                          ></v-checkbox
                        ></v-col>
                      </v-list-item>
                    </v-list>
                  </v-row>
                </v-card>
              </v-scroll-y-transition>
            </v-col>
          </v-row>

          <v-divider></v-divider>

          <v-card-actions>
            <v-btn color="blue darken-1" text @click="dialog = false">
              انصراف
            </v-btn>
            <v-btn color="blue darken-1" text @click="saveComplexRoles">
              تایید
            </v-btn>
          </v-card-actions>
        </v-card>
      </template>
    </v-dialog>
  </div>
</template>
<script>
import Vue from "vue";
import request from "@/utils/request";
// import the styles

export default Vue.extend({
  name: "AddNewUser",
  props: ["userRole"],
  components: {},
  data: () => ({
    dialog: false,
    complexRoles: [],
    active: [],
    open: [],
    selectedRoles: [],
    // define the default value
    value: null,
    // define options
  }),

  mounted() {
    this.getComplexes();
  },
  watch: {
    userRole: {
      handler() {
        this.selectedRoles = this.userRole;
      },
      deep: true,
    },
  },
  computed: {
    complexes() {
      return this.$store.state.complexes;
    },
  },

  methods: {
    saveComplexRoles() {
      this.dialog = false;
      this.$emit("updateCoplexRoles", this.selectedRoles);
    },
    getRoles() {
      this.roles = [];
      if (!this.active.length) return undefined;
      const id = this.active[0];
      request.get(`/complexRole/complex-selected-roles/${id}`).then((response) => {
      
        this.complexRoles = response.data.result;
      });
    },

    getComplexes() {
      this.$store.dispatch("getComplexes");
    },
    customFilter(item, queryText) {
      const textOne = item.nameFa.toLowerCase();
      const searchText = queryText.toLowerCase();

      return textOne.indexOf(searchText) > -1;
    },
    selectComplex() {
      return this.getRoles();
    },
  },
});
</script>
<style scoped>
/* .vue-treeselect{
  text-align: right !important;
} */
</style>
