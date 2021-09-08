import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import merge from 'deepmerge';
import { createBasicConfig } from '@open-wc/building-rollup';
import typescript from '@rollup/plugin-typescript';


const baseConfig = createBasicConfig();

export default merge(baseConfig, {
    input: './src/subless.ts',
    output: [
        {
            file: './../SublessSignIn/wwwroot/dist/subless.cjs.js',
            format: 'cjs'
        },
        {
            file: './../SublessSignIn/wwwroot/dist/subless.es.js',
            format: 'es'
        }
    ],
    // {
    //     //inlineDynamicImports: true,
    //     dir: './../SublessSignIn/wwwroot/dist/',
    //     format: 'iife',
    //     sourcemap: true,
    //     //exports: 'named',

    // },
    plugins: [
        nodeResolve({
            //ignoreGlobal: false,
            //include: ['node_modules/**'],
            dynamicRequireTargets: [
                'node_modules/oidc-client-ts/**'
            ]
        }),
        commonjs({

        }),
        typescript()
    ]
});