/* eslint-disable @typescript-eslint/no-explicit-any */
export abstract class AService {
  protected static BASE_URI = "";

  protected static getUrl(url: string): string {
    return `/api${this.BASE_URI ? "/" + this.BASE_URI : ""}/${url}`;
  }

  protected static async http(
    method: string,
    url: string,
    data?: any,
    options?: any
  ): Promise<any> {
    const headers = {
      Accept: "application/json",
      "Content-Type": "application/json",
      ...options,
    };

    const originalResponse = await fetch(this.getUrl(url), {
      method,
      body:
        method === "GET"
          ? null
          : headers["Content-Type"]
          ? JSON.stringify(data)
          : data,
      headers,
    });

    if (originalResponse.status >= 400) {
      return new Promise((resolve) => {
        const badRequestResult = originalResponse.json();

        badRequestResult.then((_) => {
          if (originalResponse.status >= 500) {
            // openNotificationError(data.message, originalResponse.status);
            return;
          }
          // openNotificationError(data.message, originalResponse.status);
        });

        resolve(badRequestResult);
      });
    }

    return new Promise((resolve, reject) => {
      if (originalResponse.headers.get("content-type") === "application/pdf") {
        return resolve(originalResponse.blob());
      }

      originalResponse.json().then((result) => {
        if (result.success === false) {
          return reject(result.message);
        }
        return resolve(result);
      });
    });
  }

  protected static get(url: string, data?: any, options?: any): any {
    const query = data ? `?${new URLSearchParams(data).toString()}` : "";
    return this.http("GET", url + query, data, options);
  }

  protected static post(url: string, data?: any, options?: any): any {
    return this.http("POST", url, data, options);
  }

  protected static put(url: string, data?: any, options?: any): any {
    return this.http("PUT", url, data, options);
  }

  protected static delete(url: string, data?: any, options?: any): any {
    return this.http("DELETE", url, data, options);
  }
}
