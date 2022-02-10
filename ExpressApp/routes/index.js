"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET home page.
 */
const fetch = require("node-fetch");
const express = require("express");
const util_1 = require("util");
const router = express.Router();
router.get('/', (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    const response = yield fetch("http:netcoreapplication");
    if (!response.ok) { /* Handle */ }
    let pageRes;
    // If you care about a response:
    if (response.body !== null) {
        // body is ReadableStream<Uint8Array>
        // parse as needed, e.g. reading directly, or
        const asString = new util_1.TextDecoder("utf-8").decode(response.body);
        // and further:
        pageRes = JSON.parse(asString); // implicitly 'any', make sure to verify type on runtime.
        console.info(pageRes);
    }
    res.render('index', { title: 'Express', text: pageRes });
}));
exports.default = router;
//# sourceMappingURL=index.js.map