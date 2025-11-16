import bg from "../assets/RideShareBackground.png";

export default function RootLayout({ children }: { children: React.ReactNode }) {
    return (
        <div
            className="min-h-screen w-screen h-full w-full bg-cover bg-center bg-no-repeat flex items-center justify-center p-4"
            style={{
                backgroundImage: `url(${bg})`,
            }}
        >
            {children}
        </div>
    );
}
