import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import merge from 'deepmerge';
import { createBasicConfig } from '@open-wc/building-rollup';
import typescript from '@rollup/plugin-typescript';


const baseConfig = createBasicConfig();

export default merge(baseConfig, {
    input: './src/test.ts',
    output: [
        // {
        //     file: './../SublessSignIn/wwwroot/dist/subless.cjs.js',
        //     format: 'cjs'
        // },
        {
            file: './../SublessSignIn/wwwroot/dist/test.es.js',
            format: 'es',
            //minifyInternalExports: false,
            exports: 'named',
        },
    ],
    // external: [
    //     'oidc-client-ts',
    //     'node_modules/oidc-client-ts/**'
    // ],
    //     //inlineDynamicImports: true,
    //     dir: './../SublessSignIn/wwwroot/dist/',
    //     format: 'iife',
    //     sourcemap: true,


    plugins: [
        nodeResolve({
            //ignoreGlobal: false,
            //include: ['node_modules/**'],
            moduleDirectories: [
                'node_modules/oidc-client-ts/**'
            ]
        }),
        commonjs({
            include: 'node_modules/**'
        }),
        typescript({
            sourceMap: true
        })
    ]
});