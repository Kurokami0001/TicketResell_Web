"use client";
import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMagnifyingGlass } from "@fortawesome/free-solid-svg-icons";
import RequestDialog from "./RequestDialog";
import ChatboxTable from "./RequestForm";

const UserRequest = () => {
  const [searchTerm, setSearchTerm] = useState<string>("");
  const chatboxData = [
    {
      ChatboxId: 1,
      Status: 0, // Pending
      CreateDate: "2024-10-29T10:00:00Z",
      Title: "Sample Chatbox Title 1",
      Description: "This is a sample description for chatbox 1.",
    },
    {
      ChatboxId: 2,
      Status: 1, // In Processing
      CreateDate: "2024-10-28T11:30:00Z",
      Title: "Sample Chatbox Title 2",
      Description: "This is a sample description for chatbox 2.",
    },
    {
      ChatboxId: 3,
      Status: 2, // Complete
      CreateDate: "2024-10-27T09:15:00Z",
      Title: "Sample Chatbox Title 3",
      Description: "This is a sample description for chatbox 3.",
    },
    {
      ChatboxId: 4,
      Status: -1, // Rejected
      CreateDate: "2024-10-26T08:45:00Z",
      Title: "Sample Chatbox Title 4",
      Description: "This is a sample description for chatbox 4.",
    },
    {
      ChatboxId: 4,
      Status: 3, // Rejected
      CreateDate: "2024-10-26T08:45:00Z",
      Title: "Sample Chatbox Title 5",
      Description: "This is a sample description for chatbox 5.",
    },
  ];
  return (
    <div className="bg-white py-12 px-10 rounded-xl ">
      <p className="text-2xl pb-10 text-center font-bold	">Request table</p>
      <div className="container mx-auto px-5 flex flex-col justify-between  sm:flex-row items-center">
        <div className="relative flex items-center bg-gray-100 mb-5 rounded-full px-4 h-12 w-full sm:w-auto ">
          <input
            type="text"
            placeholder="Search requests"
            className="border-none outline-none items-center bg-transparent w-96 text-gray-700 placeholder-gray-400 focus:ring-0"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <FontAwesomeIcon
            className="text-gray-500 cursor-pointer"
            icon={faMagnifyingGlass}
          />
        </div>
        <RequestDialog />
      </div>
      <div className="flex justify-center w-full ">
        <div className="w-full max-w-7xl">
          {/* Set a max-width for the card container */}
          <ChatboxTable chatboxData={chatboxData} />
        </div>
      </div>
    </div>
  );
};

export default UserRequest;