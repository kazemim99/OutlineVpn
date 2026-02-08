module.exports = {
  root: true,
  env: {
    node: true,
    browser: true,
    es2021: true,
  },
  extends: [
    "plugin:vue/strongly-recommended",
    "eslint:recommended",
    "@vue/typescript/recommended",
    "@vue/prettier",
  ],
  parserOptions: {
    ecmaVersion: 2021,
    parser: "@typescript-eslint/parser",
  },
  rules: {
    "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
    "@typescript-eslint/no-explicit-any": "off",
    "@typescript-eslint/no-unused-vars": ["warn", { argsIgnorePattern: "^_" }],
    "@typescript-eslint/no-empty-interface": "warn",
    "vue/multi-word-component-names": "off",
    "vue/valid-v-slot": "warn", // Vuetify 2 uses v-slot modifiers
    "vue/no-v-text-v-html-on-component": "warn",
    "vue/require-default-prop": "warn",
    "vue/require-prop-types": "warn",
    "prefer-const": "warn", // Change to warning instead of error
  },
};
