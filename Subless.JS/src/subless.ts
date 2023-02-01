import {Subless, HitStrategy} from "./subless2.0";

/** Provides a default subless instance with URI hit tracking enabled
 * @return {Subless} A subless instance
 */
export default function SublessInstance(): Subless {
    return new Subless(HitStrategy.uri);
}
