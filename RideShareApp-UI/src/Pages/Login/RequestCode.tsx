import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/Components/ui/button";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/Components/ui/tabs";
import {requestCodeByEmail, requestCodeByPhoneNumber } from "@/Services/Api";

// ----- Validation schema -----
const contactSchema = z.discriminatedUnion("method", [
    z.object({
        method: z.literal("email"),
        email: z.string().email({ message: "Please enter a valid email address." }),
    }),
    z.object({
        method: z.literal("phone"),
        phone: z
            .string()
            .min(10, { message: "Please enter a valid phone number." })
            .max(20)
            .regex(/^\+?[0-9\-()\s]+$/, { message: "Phone number contains invalid characters." }),
    }),
]);

type ContactFormData = z.infer<typeof contactSchema>;

/**
 * ContactAuthForm
 *
 * A modern, accessible React + TypeScript form (Tailwind-friendly) that lets the user
 * choose between Email or Phone and request a verification code.
 *
 * Dependency notes: install react-hook-form, zod, @hookform/resolvers
 *   npm install react-hook-form zod @hookform/resolvers
 *
 * Tailwind: this component uses utility classes from Tailwind for styling. If you
 * don't use Tailwind, replace className values with your preferred styles.
 */
export function LoginForm() {
    const [sentMessage, setSentMessage] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [isSending, setIsSending] = useState(false);

    const {
        register,
        handleSubmit,
        watch,
        setValue,
        formState: { errors, isSubmitting },
        reset,
    } = useForm<ContactFormData>({
        resolver: zodResolver(contactSchema),
        defaultValues: { method: "email", email: "", phone: "" } as any,
    });
    
    

    const method = watch("method") as "email" | "phone";

    async function onSubmit(data: ContactFormData) {
        setErrorMessage(null);
        setSentMessage(null);
        setIsSending(true);

        try {
            const response = data.method === "phone" ? 
                await requestCodeByPhoneNumber({number: data.phone}) : 
                await requestCodeByEmail({email: data.email});

            setSentMessage(
                method === "email"
                    ? "A verification code has been sent to your email."
                    : "A verification code has been sent to your phone number."
            );

            // Optionally clear the contact input but keep method
            reset({ method: data.method, email: "", phone: "" } as any);
        } catch (err: any) {
            const apiMessage = err?.response?.data?.message;
            const status = err?.response?.status;

            throw new Error(apiMessage || `Request failed (${status})`);
            // setErrorMessage(err?.message || "Unable to send code. Please try again.");
        } finally {
            setIsSending(false);
        }
        
        // console.log("Sending the code to email or phone.", data);
    }

    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            <div className="w-full max-w-md bg-white rounded-2xl shadow-md border border-gray-100">
                <div className="p-6">
                    <h1 className="text-2xl font-semibold text-gray-900 mb-2">Let's Verify</h1>
                    <br />
                    <p className="text-sm text-gray-500 mb-4">
                        Enter your email or phone number to receive a sign-in code. Choose which contact
                        method you'd like to use.
                    </p>
                    <br />

                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                        {/* Method switch */}
                        <Tabs
                            defaultValue="email"
                            onValueChange={(value) => setValue("method", value)} // sync with RHF
                            className="w-full"
                        >
                            <TabsList className="grid w-full grid-cols-2">
                                <TabsTrigger value="email">Email</TabsTrigger>
                                <TabsTrigger value="phone">Phone</TabsTrigger>
                            </TabsList>

                            {/* Email Tab */}
                            <TabsContent value="email" className="mt-4">
                                <div>
                                    {/*<label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="email">
                                        Email address
                                    </label>*/}
                                    <input
                                        id="email"
                                        type="email"
                                        placeholder="you@example.com"
                                        aria-invalid={errors.email ? "true" : "false"}
                                        {...register("email")}
                                        className={`block w-full rounded-md border px-3 py-2 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                                            errors.email ? "border-red-300" : "border-gray-200"
                                        }`}
                                    />
                                    {errors.email && (
                                        <p className="mt-1 text-sm text-red-600">{errors.email.message as string}</p>
                                    )}
                                </div>
                            </TabsContent>

                            {/* Phone Tab */}
                            <TabsContent value="phone" className="mt-4">
                                <div>
                                    {/*<label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="phone">
                                        Phone number
                                    </label>*/}
                                    <input
                                        id="phone"
                                        type="tel"
                                        placeholder="+1 555 555 5555"
                                        aria-invalid={errors.phone ? "true" : "false"}
                                        {...register("phone")}
                                        className={`block w-full rounded-md border px-3 py-2 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                                            errors.phone ? "border-red-300" : "border-gray-200"
                                        }`}
                                    />
                                    {errors.phone && (
                                        <p className="mt-1 text-sm text-red-600">{errors.phone.message as string}</p>
                                    )}
                                </div>
                            </TabsContent>
                        </Tabs>

                        {/* Submit section */}
                        <div className="pt-2">
                            <Button
                                type="submit"
                                variant="default"
                                disabled={isSubmitting || isSending}
                            >
                                {isSending || isSubmitting ? "Sending code..." : "Send code"}
                            </Button>
                        </div>

                        {/* Status messages */}
                        {sentMessage && <p className="text-sm text-green-600">{sentMessage}</p>}
                        {errorMessage && <p className="text-sm text-red-600">{errorMessage}</p>}

                        <p className="text-xs text-gray-400 mt-2">
                            By continuing, you agree to our terms. The verification code will expire after a short
                            time.
                        </p>
                    </form>
                </div>
            </div>
        </div>
    );
}
