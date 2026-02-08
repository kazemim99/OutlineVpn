<template>
  <nav>
    <ol class="breadcrumb">
      <li
        v-for="(crumb, ci) in crumbs"
        :key="ci"
        class="breadcrumb-item align-items-center"
      >
        <button
          class="btn btn-link"
          :class="{ disabled: isLast(ci) }"
          @click="selected(crumb)"
        >
          {{ crumb }}
        </button>
      </li>
    </ol>
  </nav>
</template>
<template>
  <div>
    <v-breadcrumbs :items="crumbs"></v-breadcrumbs>
  </div>
</template>
<script>
export default {
  name: "Breadcrump",
  props: {
    crumbs: {
      text: "",
      disabled: false,
      href: "",
    },
  },
};
</script>
