<template>
  <v-row justify="center">
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      persistent
      max-width="600px"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on">افزودن</v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">نقش جدید</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    outlined
                    clearable
                    v-model="role.title"
                    label="نام  *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="titleRules"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="6" sm="12" md="6">
                  <v-text-field
                    outlined
                    clearable
                    v-model="role.appearanceTitle"
                    label="نام نمایشی *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="appearanceTitleRules"
                    required
                  ></v-text-field>
                </v-col>
                <v-col>
                  <v-treeview
                    v-model="role.selectedPermissions"
                    selected-color="red"
                    selectable
                    :items="permissions"
                  ></v-treeview>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">بستن</v-btn>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit"
            >ذخیره</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "AddNewComplexRole",
  components: {},
  data: () => ({
    roleId: null,
    permissions: [],
    dialog: false,
    valid: true,
    loading: false,
    complexRoleId: null,
    role: {
      appearanceTitle: "",
      title: "",
      selectedPermissions: [],
      complexId: null,
    },

    titleRules: [(v) => !!v || "لطفا عنوان  را وارد نمایید"],
    appearanceTitleRules: [(v) => !!v || "لطفا عنوان نمایشی را وارد نمایید"],
  }),
  mounted() {
    this.role.complexId = this.$route.params.id;
    this.getPermissions();
  },
  computed: {},
  watch: {
    dialog: {
      handler() {
        if (this.dialog) {
          if (this.complexRoleId) this.getComplex(this.complexRoleId);
        } else {
          (this.role.title = ""),
            (this.role.nameFa = ""),
            (this.role.appearanceTitle = ""),
            (this.complexRoleId = null),
            (this.role.selectedPermissions = []),
            (this.complexId = null);
        }
      },
      deep: true,
    },
  },
  methods: {
    async getComplex(id) {
      await request.get(`/complexRole/${id}`).then((response) => {
        var data = response.data.result;
        this.role.title = data.title;
        this.role.appearanceTitle = data.appearanceTitle;
        this.role.selectedPermissions = data.permissions;
      });
    },
    async getPermissions() {
      await request.get(`/complexRole/permmissions`).then((response) => {
        this.permissions = response.data.result;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      if (this.complexRoleId) {
        request
          .put(`/complexRole/${this.complexRoleId}`, this.role)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadRoles");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/complexRole", this.role)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadRoles");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },
  },
});
</script>

<style scoped>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.5;
  position: absolute;
  width: 100%;
}

.card-form-img {
  padding: 0px !important;
}

.icon-btn-modal {
  position: absolute;
  font-size: 18px !important;
  color: #fff !important;
  padding: 8px;
  border-radius: 50%;
}

.icon-btn-modal:hover {
  cursor: pointer;
}

.icon-btn-upload {
  position: absolute;
  left: 60%;
  bottom: 33%;
  color: #fff !important;
  /*padding: 8px;*/
  border-radius: 50%;
  /*background: #35495E !important;*/
  text-align: center;
  display: flex;
  margin: auto;
  justify-content: center;
  align-items: center;
  /*height: 20px !important;*/
  /*width: 20px !important;*/
}

.v-icon {
  color: #fff !important;
  font-size: 18px !important;
  text-align: center;
  background: #35495e !important;
}

.logo-title {
  text-align: center;
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}
</style>
