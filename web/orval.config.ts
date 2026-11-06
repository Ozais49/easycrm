import { defineConfig } from 'orval'

export default defineConfig({
  cem: {
    input: './openapi/cem.json',
    output: {
      mode: 'tags-split',
      target: './src/lib/api/generated/endpoints',
      schemas: './src/lib/api/generated/models',
      client: 'react-query',
      httpClient: 'axios',
      clean: true,
      override: {
        mutator: { path: './src/lib/api/client.ts', name: 'apiFetch' },
      },
    },
  },
})
